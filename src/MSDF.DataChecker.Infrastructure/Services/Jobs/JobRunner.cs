// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Providers;
using MSDF.DataChecker.Domain.Resources;
using Rule = MSDF.DataChecker.Domain.Entities.Rule;

namespace MSDF.DataChecker.Domain.Services.Jobs
{
    public interface IJobRunner
    {
        void Run(JobResource job);

        Task RunAsync(JobResource job, CancellationToken cancellationToken);
    }

    public class JobRunner : IJobRunner
    {
        private readonly IDatabaseEnvironmentConnectionStringProvider _databaseEnvironmentConnectionStringProvider;
        private readonly DatabaseContext _db;

        public JobRunner(DatabaseContext db,
            IDatabaseEnvironmentConnectionStringProvider databaseEnvironmentConnectionStringProvider)
        {
            _db = db;
            _databaseEnvironmentConnectionStringProvider = databaseEnvironmentConnectionStringProvider;
        }

        public void Run(JobResource job)
        {
            RunAsync(job, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task RunAsync(JobResource job, CancellationToken cancellationToken)
        {
            if (job?.DatabaseEnvironmentId == null)
            {
                return;
            }

            List<Rule> toRun = new List<Rule>();

            var databaseEnvironment = await _db.DatabaseEnvironments
                .Include(rec => rec.UserParams)
                .SingleOrDefaultAsync(rec => rec.Id == job.DatabaseEnvironmentId.Value, cancellationToken);

            var collections = new List<Guid>();
            var containers = new List<Guid>();
            var tags = new List<int>();

            switch (job.Type)
            {
                case JobResource.JobType.Tag:
                    if (!job.TagId.HasValue)
                    {
                        return;
                    }

                    tags.Add(job.TagId.Value);
                    break;
                case JobResource.JobType.Container:
                    var containerType = await _db.Containers
                        .Include(m => m.ChildContainers)
                        .SingleOrDefaultAsync(m => m.Id == job.ContainerId.Value, cancellationToken);

                    if (containerType.ContainerTypeId == 1)
                    {
                        collections.Add(job.ContainerId.Value);
                    }
                    else
                    {
                        containers.Add(job.ContainerId.Value);
                    }

                    break;
            }

            var result = await SearchRulesAsync(collections, containers, tags, string.Empty, string.Empty, null, null);

            toRun.AddRange(result.Rules);
            toRun = toRun.Where(r => r.Id != Guid.Empty).ToList();

            foreach (var rule in toRun)
            {
                await ExecuteRuleByEnvironmentIdAsync(rule, databaseEnvironment);
            }

            async Task ExecuteRuleByEnvironmentIdAsync(Rule rule, DatabaseEnvironment databaseEnvironment)
            {
                int? ruleDetailsDestinationId = null;

                var executionLogs = await _db.RuleExecutionLogs
                    .Where(m => m.RuleId == rule.Id)
                    .OrderByDescending(m => m.ExecutionDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var connectionString =
                    await _databaseEnvironmentConnectionStringProvider.GetConnectionString(databaseEnvironment);

                var stopWatch = Stopwatch.StartNew();

                var testResult = await ExecuteRuleAsync(
                    rule, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes);

                var containerParent = await _db.Containers
                    .Include(m => m.ChildContainers)
                    .SingleOrDefaultAsync(m => m.Id == rule.ContainerId, cancellationToken);

                if (containerParent.ParentContainerId.HasValue)
                {
                    var collectionParent = await _db.Containers
                        .Include(m => m.ChildContainers)
                        .SingleOrDefaultAsync(m => m.Id == containerParent.ParentContainerId.Value, cancellationToken);

                    ruleDetailsDestinationId = collectionParent.RuleDetailsDestinationId;
                }

                RuleExecutionLog ruleExecutionLog = new RuleExecutionLog()
                {
                    Id = 0,
                    Evaluation = testResult.Evaluation,
                    RuleId = rule.Id,
                    StatusId = (int) testResult.Status,
                    Result = testResult.Result,
                    Response = testResult.Evaluation
                        ? "Ok"
                        : testResult.ErrorMessage,
                    DatabaseEnvironmentId = databaseEnvironment.Id,
                    ExecutedSql = testResult.ExecutedSql,
                    DiagnosticSql = rule.DiagnosticSql,
                    RuleDetailsDestinationId = ruleDetailsDestinationId
                };

                if (ruleExecutionLog.RuleDetailsDestinationId == null || ruleExecutionLog.RuleDetailsDestinationId.Value == 0)
                {
                    ruleExecutionLog.RuleDetailsDestinationId = null;
                }

                testResult.LastExecuted = executionLogs.Any()
                    ? executionLogs.FirstOrDefault().ExecutionDate
                    : (DateTime?) null;

                ruleExecutionLog.ExecutionTimeMs = stopWatch.ElapsedMilliseconds;
                ruleExecutionLog.Result = testResult.Result;

                ruleExecutionLog.ExecutionDate = DateTime.UtcNow;

                var newRuleExecutionLog = await _db.AddAsync(ruleExecutionLog, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                testResult.TestResults = await _db.RuleExecutionLogs
                    .Where(m => m.RuleId == rule.Id && m.DatabaseEnvironmentId == databaseEnvironment.Id)
                    .OrderByDescending(m => m.ExecutionDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                if (!testResult.Evaluation)
                {
                    testResult.DiagnosticSql = rule.DiagnosticSql;
                }

                try
                {
                    //Validate if the rule is going to any queue table
                    if (ruleDetailsDestinationId != null && ruleDetailsDestinationId.Value > 0)
                    {
                        var existCatalog = await _db.Catalogs
                            .SingleOrDefaultAsync(rec => rec.Id == ruleDetailsDestinationId.Value, cancellationToken);

                        if (existCatalog != null)
                        {
                            int maxNumberResults = databaseEnvironment.MaxNumberResults.Value;

                            if (rule.MaxNumberResults != null)
                            {
                                maxNumberResults = rule.MaxNumberResults.Value;
                            }

                            await InsertDiagnosticSqlIntoDetails(
                                rule, newRuleExecutionLog.Entity, connectionString, databaseEnvironment.UserParams,
                                existCatalog.Name,
                                maxNumberResults);
                        }
                    }
                }
                catch (Exception ex)
                {
                    newRuleExecutionLog.Entity.Result = -1;
                    newRuleExecutionLog.Entity.Response = ex.Message;
                    newRuleExecutionLog.Entity.ExecutionDate = DateTime.UtcNow;
                    _db.RuleExecutionLogs.Update(newRuleExecutionLog.Entity);
                }

                await _db.SaveChangesAsync(cancellationToken);
            }

            async Task InsertDiagnosticSqlIntoDetails(Rule rule, RuleExecutionLog ruleExecutionLog, string connectionString,
                List<UserParam> sqlParams, string tableName, int maxNumberResults)
            {
                string sqlToRun = GenerateSqlWithTop(rule.DiagnosticSql, maxNumberResults.ToString());
                string columnsSchema = string.Empty;
                var listColumnsFromDestination = await GetColumnsByTableAsync(tableName, "destination");

                await using var conn = new SqlConnection(connectionString);

                await conn.OpenAsync(cancellationToken);

                await using (var cmd = new SqlCommand(sqlToRun, conn))
                {
                    AddParameters(sqlToRun, cmd, sqlParams);
                    var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                    Dictionary<string, string> listColumns = new Dictionary<string, string>();
                    listColumnsFromDestination.ForEach(rec => listColumns.Add(rec.Name, rec.Type));

                    DataTable tableToInsert = GetTableForSqlBulk(ruleExecutionLog.Id, reader, listColumns, out columnsSchema);

                    if (tableToInsert != null && tableToInsert.Rows.Count > 0)
                    {
                        if (ruleExecutionLog != null)
                        {
                            ruleExecutionLog.DetailsSchema = columnsSchema;
                            ruleExecutionLog.ExecutionDate = DateTime.UtcNow;
                            await _db.AddAsync(ruleExecutionLog, cancellationToken);
                            await _db.SaveChangesAsync(cancellationToken);
                        }

                        await ExecuteSqlBulkCopy(tableToInsert, $"[destination].[{tableName}]");
                    }
                }

                async Task ExecuteSqlBulkCopy(DataTable table, string tableName)
                {
                    string connectionString = _db.Database.GetDbConnection().ConnectionString;

                    await using SqlConnection destinationConnection = new SqlConnection(connectionString);

                    await destinationConnection.OpenAsync(cancellationToken);
                    using SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection) {DestinationTableName = tableName};

                    await bulkCopy.WriteToServerAsync(table, cancellationToken);
                }

                static DataTable GetTableForSqlBulk(int ruleExecutionLogId,
                    SqlDataReader reader,
                    Dictionary<string, string> columns,
                    out string columnsSchema)
                {
                    Dictionary<string, string> listColumnsSchema = new Dictionary<string, string>();
                    DataTable table = new DataTable();
                    List<string> classColumns = columns.Select(rec => rec.Key).ToList();
                    columnsSchema = string.Empty;

                    foreach (var column in columns)
                    {
                        if (column.Value.Contains("int"))
                        {
                            table.Columns.Add(column.Key, typeof(int));
                        }
                        else if (column.Value.Contains("varchar"))
                        {
                            table.Columns.Add(column.Key, typeof(string));
                        }
                        else if (column.Value.Contains("date"))
                        {
                            table.Columns.Add(column.Key, typeof(DateTime));
                        }
                        else if (column.Value.Contains("decimal") || column.Value.Contains("numeric"))
                        {
                            table.Columns.Add(column.Key, typeof(decimal));
                        }
                        else if (column.Value.Contains("float"))
                        {
                            table.Columns.Add(column.Key, typeof(double));
                        }
                    }

                    if (reader.HasRows)
                    {
                        List<string> staticColumns = new List<string>();
                        List<string> jsonColumns = new List<string>();

                        var columnsSource = reader.GetColumnSchema();

                        foreach (var column in columnsSource)
                        {
                            string columnName = column.ColumnName.ToLower();

                            if (classColumns.Contains(columnName))
                            {
                                staticColumns.Add(columnName);
                            }
                            else
                            {
                                jsonColumns.Add(columnName);
                            }

                            if (column.DataTypeName.ToLower().Contains("date"))
                            {
                                listColumnsSchema.Add(column.ColumnName, "datetime");
                            }
                            else
                            {
                                listColumnsSchema.Add(column.ColumnName, "string");
                            }
                        }

                        while (reader.Read())
                        {
                            DataRow dr = table.NewRow();
                            dr["ruleexecutionlogid"] = ruleExecutionLogId;

                            foreach (string column in staticColumns)
                            {
                                dr[column] = reader.GetValue(column).ToString();
                            }

                            // TODO : JSON NEEDS FIXING
                            // JsonDocument jsonObject = new JsonDocument();
                            // foreach (string column in jsonColumns)
                            // {
                            //     string strNewValue = reader.GetValue(column).ToString();
                            //     jsonObject.Add(column, strNewValue);
                            // }

                            // dr["otherdetails"] = jsonObject.ToString();
                            table.Rows.Add(dr);
                        }
                    }

                    columnsSchema = JsonSerializer.Serialize(listColumnsSchema);
                    return table;
                }

                async Task<List<DestinationTableColumn>> GetColumnsByTableAsync(string tableName, string tableSchema)
                {
                    List<DestinationTableColumn> columns = new List<DestinationTableColumn>();

                    string connectionString = _db.Database.GetDbConnection().ConnectionString;

                    await using SqlConnection destinationConnection = new SqlConnection(connectionString);

                    await destinationConnection.OpenAsync(cancellationToken);

                    string sql = string.Format(
                        "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE " +
                        "FROM INFORMATION_SCHEMA.COLUMNS " +
                        "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema " +
                        "ORDER BY ORDINAL_POSITION");

                    await using var sqlCommand = new SqlCommand(sql, destinationConnection);

                    sqlCommand.Parameters.AddWithValue("@tablename", tableName);
                    sqlCommand.Parameters.AddWithValue("@tableschema", tableSchema);

                    var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        columns.Add(
                            new DestinationTableColumn
                            {
                                Name = reader.GetValue(0).ToString().ToLower(),
                                Type = reader.GetValue(1).ToString().ToLower(),
                                IsNullable = reader.GetValue(2).ToString().ToLower() != "no"
                            });
                    }

                    return columns;
                }

                string GenerateSqlWithTop(string diagnosticSql, string maxNumberResults)
                {
                    string result = diagnosticSql;
                    result = result.ToLower().Trim();

                    if (result.StartsWith("select"))
                    {
                        if (!result.StartsWith("select top"))
                        {
                            if (result.StartsWith("select distinct"))
                            {
                                var regex = new Regex(Regex.Escape("select distinct"));
                                result = regex.Replace(result, "select distinct top " + maxNumberResults + " ", 1);
                            }
                            else
                            {
                                var regex = new Regex(Regex.Escape("select"));
                                result = regex.Replace(result, "select top " + maxNumberResults + " ", 1);
                            }
                        }
                    }
                    else if (result.StartsWith("with"))
                    {
                        if (!result.Contains(") select top"))
                        {
                            if (result.Contains(") select distinct"))
                            {
                                var regex = new Regex(Regex.Escape(") select distinct"));
                                result = regex.Replace(result, ") select distinct top " + maxNumberResults + " ", 1);
                            }
                            else
                            {
                                var regex = new Regex(Regex.Escape(") select"));
                                result = regex.Replace(result, ") select top " + maxNumberResults + " ", 1);
                            }
                        }
                    }

                    return result;
                }
            }

            static void AddParameters(string sql, SqlCommand sqlCommand, List<UserParam> parameters)
            {
                parameters?.ForEach(
                    m =>
                    {
                        if (sql.Contains("@" + m.Name))
                        {
                            sqlCommand.Parameters.AddWithValue("@" + m.Name, m.Value);
                        }
                    });
            }

            static string GenerateSqlWithCount(string diagnosticSql)
            {
                string result = diagnosticSql;
                result = result.ToLower().Trim().Replace(";", "");

                if (result.StartsWith("select"))
                {
                    return string.Format("SELECT COUNT(*) FROM ( \n {0} \n) as TBL", result);
                }

                return string.Empty;
            }

            async Task<RuleTestResult> ExecuteRuleAsync(Rule rule, string connectionString, List<UserParam> userParams,
                int? timeout)
            {
                var stopWatch = Stopwatch.StartNew();
                RuleTestResult testResult;

                try
                {
                    if (!connectionString.ToLower().Contains("timeout") && timeout == null)
                    {
                        connectionString += " Connection Timeout = 60";
                    }
                    else if (timeout != null)
                    {
                        connectionString += " Connection Timeout = " + (timeout.Value * 60).ToString();
                    }

                    await using var conn = new SqlConnection(connectionString);

                    int execution = 0;
                    bool resultWithErrors = false;
                    await conn.OpenAsync(cancellationToken);
                    string sqlToRun = GenerateSqlWithCount(rule.DiagnosticSql);

                    try
                    {
                        if (string.IsNullOrEmpty(sqlToRun))
                        {
                            sqlToRun = rule.DiagnosticSql;

                            await using var cmd = new SqlCommand(sqlToRun, conn);

                            if (timeout != null)
                            {
                                cmd.CommandTimeout = timeout.Value * 60;
                            }

                            AddParameters(sqlToRun, cmd, userParams);
                            var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync(cancellationToken))
                                {
                                    execution++;
                                }
                            }
                        }
                        else
                        {
                            await using var cmd = new SqlCommand(sqlToRun, conn);

                            if (timeout != null)
                            {
                                cmd.CommandTimeout = timeout.Value * 60;
                            }

                            AddParameters(sqlToRun, cmd, userParams);
                            execution = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
                        }
                    }
                    catch
                    {
                        sqlToRun = rule.DiagnosticSql;

                        await using var cmd = new SqlCommand(sqlToRun, conn);

                        if (timeout != null)
                        {
                            cmd.CommandTimeout = timeout.Value * 60;
                        }

                        AddParameters(sqlToRun, cmd, userParams);
                        var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                execution++;
                            }
                        }
                    }

                    resultWithErrors = execution > 0;

                    testResult = new RuleTestResult
                    {
                        Id = 0,
                        Rule = rule,
                        Result = execution,
                        Evaluation = !resultWithErrors,
                        Status = !resultWithErrors
                            ? Status.Succeeded
                            : Status.Failed,
                        ErrorMessage = !resultWithErrors
                            ? ""
                            : rule.ErrorMessage,
                        ExecutedSql = rule.DiagnosticSql
                    };
                }
                catch (Exception e)
                {
                    testResult = new RuleTestResult()
                    {
                        Rule = rule,
                        Result = -1,
                        Evaluation = false,
                        Status = Status.Error,
                        ErrorMessage = e.Message,
                        ExecutedSql = rule.DiagnosticSql
                    };
                }

                stopWatch.Stop();
                testResult.ExecutionTimeMs = stopWatch.ElapsedMilliseconds;
                return testResult;
            }

            async Task<SearchResults> SearchRulesAsync(List<Guid> collectionsSelected, List<Guid> containersSelected,
                List<int> tagsSelected, string name, string diagnostic, int? detailsDestination, int? severityId)
            {
                var result = new SearchResults();

                List<Rule> allRules = await GetByCollectionOrContainerOrTagAsync(
                    collectionsSelected, containersSelected, tagsSelected.ToList());

                if (!string.IsNullOrEmpty(name))
                {
                    allRules = allRules
                        .Where(rec => rec.Name.ToLower().Contains(name.ToLower()))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(diagnostic))
                {
                    allRules = allRules
                        .Where(rec => rec.DiagnosticSql.ToLower().Contains(diagnostic.ToLower()))
                        .ToList();
                }

                if (detailsDestination != null)
                {
                    var collectionsByDestination = await _db.Containers
                        .Where(
                            m => m.RuleDetailsDestinationId != null &&
                                 m.RuleDetailsDestinationId.Value == detailsDestination.Value)
                        .Include(m => m.ChildContainers)
                        .ToListAsync(cancellationToken);

                    if (collectionsByDestination != null && collectionsByDestination.Any())
                    {
                        List<Guid> containersByDestination = new List<Guid>();

                        foreach (var collection in collectionsByDestination)
                        {
                            if (collection.ChildContainers != null && collection.ChildContainers.Any())
                            {
                                containersByDestination.AddRange(collection.ChildContainers.Select(rec => rec.Id));
                            }
                        }

                        if (containersByDestination.Any())
                        {
                            containersByDestination = containersByDestination.Distinct().ToList();

                            allRules = allRules
                                .Where(rec => containersByDestination.Contains(rec.ContainerId))
                                .ToList();
                        }
                    }
                }

                if (severityId != null)
                {
                    allRules = allRules
                        .Where(rec => rec.ErrorSeverityLevel == severityId.Value)
                        .ToList();
                }

                result.Rules = allRules;

                if (result.Rules != null && result.Rules.Count > 0)
                {
                    var listEnvironmentType = await _db.Catalogs
                        .Where(rec => rec.CatalogType == "EnvironmentType")
                        .ToListAsync(cancellationToken);

                    List<Guid> containersParents = result
                        .Rules
                        .Select(rec => rec.ContainerId)
                        .ToList();

                    containersParents = containersParents.Distinct().ToList();

                    List<Container> allContainersParents = await _db.Containers
                        .Where(
                            m => containersParents.Contains(m.Id) || m.ParentContainerId != null &&
                                containersParents.Contains(m.ParentContainerId.Value))
                        .Include(m => m.ChildContainers)
                        .ToListAsync(cancellationToken);

                    var parentContainerIds = allContainersParents
                        .Where(x => x.ParentContainerId.HasValue)
                        .Select(rec => rec.ParentContainerId)
                        .ToList();

                    List<Container> allContainersMain = await _db.Containers
                        .Where(
                            m => parentContainerIds.Contains(m.Id) || m.ParentContainerId != null &&
                                parentContainerIds.Contains(m.ParentContainerId.Value))
                        .Include(m => m.ChildContainers)
                        .ToListAsync(cancellationToken);

                    // TODO is this really needed?
                    // foreach (var rule in result.Rules)
                    // {
                    //     Container existParent = allContainersParents.FirstOrDefault(rec => rec.Id == rule.ContainerId);
                    //
                    //     if (existParent != null && existParent.ParentContainerId != null)
                    //     {
                    //         Container existParentMain =
                    //             allContainersMain.FirstOrDefault(rec => rec.Id == existParent.ParentContainerId.Value);
                    //
                    //         if (existParentMain != null)
                    //         {
                    //             Catalog existType =
                    //                 listEnvironmentType.FirstOrDefault(rec => rec.Id == existParentMain.EnvironmentType);
                    //
                    //             if (existType != null)
                    //             {
                    //                 rule.ParentContainer = existParentMain.Id;
                    //                 rule.EnvironmentTypeText = existType.Name;
                    //                 rule.CollectionContainerName = $"{existParentMain.Name}/{existParent.Name}";
                    //                 rule.ContainerName = existParent.Name;
                    //                 rule.CollectionName = existParentMain.Name;
                    //             }
                    //         }
                    //     }
                    // }

                    // result.Rules = result.Rules.OrderBy(rec => rec.).ToList();
                }

                return result;
            }

            async Task<List<Rule>> GetByCollectionOrContainerOrTagAsync(List<Guid> collections, List<Guid> containers,
                List<int> tags)
            {
                List<Rule> result = new List<Rule>();

                var rules = await (from r in _db.Rules
                    join c in _db.Containers on r.ContainerId equals c.Id
                    join p in _db.Containers on c.ParentContainerId equals p.Id
                    where (collections.Count == 0 || collections.Contains(p.Id)) &&
                          (containers.Count == 0 || containers.Contains(c.Id))
                    select new
                    {
                        Rule = r,
                        RuleParent = c,
                        ContainerParent = p
                    }).ToListAsync(cancellationToken);

                if (tags.Any())
                {
                    List<Guid> rulesByTag = _db.TagEntities
                        .Where(rec => rec.RuleId != null && tags.Contains(rec.TagId))
                        .Select(rec => rec.RuleId.Value)
                        .ToList();

                    List<Guid> containersByTag = _db.TagEntities
                        .Where(rec => rec.ContainerId != null && tags.Contains(rec.TagId))
                        .Select(rec => rec.ContainerId.Value)
                        .ToList();

                    rules = rules
                        .Where(
                            rec => containersByTag.Contains(rec.ContainerParent.Id) ||
                                   containersByTag.Contains(rec.RuleParent.Id) || rulesByTag.Contains(rec.Rule.Id))
                        .ToList();
                }

                if (rules.Any())
                {
                    result = rules.Select(rec => rec.Rule).ToList();
                }

                return result;
            }
        }

        private class SearchResults
        {
            public List<Tag> TagsSelected { get; set; }

            public List<Container> Collections { get; set; }

            public List<Container> Containers { get; set; }

            public List<Rule> Rules { get; set; }
        }
    }
}
