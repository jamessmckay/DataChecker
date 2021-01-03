// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Providers;

namespace MSDF.DataChecker.Domain.Services
{
    public interface IRuleRunner
    {
        Task<RuleTestResult> ExecuteRuleAsync(Rule rule, DatabaseEnvironment environment);
    }

    public class RuleRunner : IRuleRunner
    {
        private readonly IDatabaseEnvironmentConnectionStringProvider _connectionStringProvider;
        private readonly ILogger<RuleRunner> _logger;
        private readonly IMapper _mapper;

        public RuleRunner(ILogger<RuleRunner> logger, IDatabaseEnvironmentConnectionStringProvider connectionStringProvider,
            IMapper mapper)
        {
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
            _mapper = mapper;
        }

        public async Task<RuleTestResult> ExecuteRuleAsync(Rule rule, DatabaseEnvironment environment)
        {
            string connectionString = await _connectionStringProvider.GetConnectionString(environment);
            RuleTestResult testResult;
            var sw = Stopwatch.StartNew();

            try
            {
                int? timeout = environment.TimeoutInMinutes;
                var userParams = environment.UserParams;

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
                await conn.OpenAsync();
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
                        var reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
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
                        execution = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    sqlToRun = rule.DiagnosticSql;

                    await using var cmd = new SqlCommand(sqlToRun, conn);

                    if (timeout != null)
                    {
                        cmd.CommandTimeout = timeout.Value * 60;
                    }

                    AddParameters(sqlToRun, cmd, userParams);
                    var reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
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
                testResult = new RuleTestResult
                {
                    Rule = rule,
                    Result = -1,
                    Evaluation = false,
                    Status = Status.Failed,
                    ErrorMessage = e.Message,
                    ExecutedSql = rule.DiagnosticSql
                };
            }

            sw.Stop();
            testResult.ExecutionTimeMs = sw.ElapsedMilliseconds;
            return testResult;
        }

        public static string GenerateSqlWithCount(string diagnosticSql)
        {
            string result = diagnosticSql;
            result = result.ToLower().Trim().Replace(";", "");

            if (result.StartsWith("select"))
            {
                return string.Format("SELECT COUNT(*) FROM ( \n {0} \n) as TBL", result);
            }

            return string.Empty;
        }

        private void AddParameters(string sql, SqlCommand sqlCommand, List<UserParam> parameters)
        {
            if (parameters != null)
            {
                parameters.ForEach(
                    m =>
                    {
                        if (sql.Contains("@" + m.Name))
                        {
                            sqlCommand.Parameters.AddWithValue("@" + m.Name, m.Value);
                        }
                    });
            }
        }

        //
        // public async Task<RuleTestResult> ExecuteRuleByEnvironmentIdAsync(Guid ruleId,DatabaseEnvironmentBO databaseEnvironment)
        // {
        //     int? ruleDetailsDestinationId = null;
        //     var rule = await _ruleService.GetAsync(ruleId);
        //     var executionLogs = await _ruleExecutionLogQueries.GetByRuleIdAsync(ruleId);
        //
        //     var connectionString = databaseEnvironment.GetConnectionString();
        //
        //     var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        //
        //     RuleTestResult testResult = await ExecuteRuleAsync(rule, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes);
        //
        //     var containerParent = await _collectionQueries.GetAsync(rule.ContainerId);
        //     if (containerParent.ParentContainerId != null)
        //     {
        //         var collectionParent = await _collectionQueries.GetAsync(containerParent.ParentContainerId.Value);
        //         ruleDetailsDestinationId = collectionParent.RuleDetailsDestinationId;
        //     }
        //
        //     RuleExecutionLog ruleExecutionLog = new RuleExecutionLog()
        //     {
        //         Id = 0,
        //         Evaluation = testResult.Evaluation,
        //         RuleId = ruleId,
        //         StatusId = (int)testResult.Status,
        //         Result = testResult.Result,
        //         Response = testResult.Evaluation ? "Ok" : testResult.ErrorMessage,
        //         DatabaseEnvironmentId = databaseEnvironment.Id,
        //         ExecutedSql = testResult.ExecutedSql,
        //         DiagnosticSql = rule.DiagnosticSql,
        //         RuleDetailsDestinationId = ruleDetailsDestinationId
        //     };
        //
        //     if (ruleExecutionLog.RuleDetailsDestinationId == null || ruleExecutionLog.RuleDetailsDestinationId.Value == 0)
        //         ruleExecutionLog.RuleDetailsDestinationId = null;
        //
        //     testResult.LastExecuted = executionLogs.Any() ? executionLogs.FirstOrDefault().ExecutionDate : (DateTime?)null;
        //     ruleExecutionLog.ExecutionTimeMs = stopWatch.ElapsedMilliseconds;
        //     ruleExecutionLog.Result = testResult.Result;
        //
        //     var newRuleExecutionLog = await _ruleExecutionLogCommands.AddAsync(ruleExecutionLog);
        //     testResult.TestResults = await _ruleService.GetTopResults(ruleId,databaseEnvironment.Id);
        //
        //     if (!testResult.Evaluation)
        //     {
        //         testResult.DiagnosticSql = rule.DiagnosticSql;
        //     }
        //
        //     try
        //     {
        //         //Validate if the rule is going to any queue table
        //         if (ruleDetailsDestinationId != null && ruleDetailsDestinationId.Value > 0)
        //         {
        //             var existCatalog = await _catalogQueries.GetAsync(ruleDetailsDestinationId.Value);
        //             if (existCatalog != null)
        //             {
        //                 int maxNumberResults = databaseEnvironment.MaxNumberResults.Value;
        //                 if (rule.MaxNumberResults != null)
        //                     maxNumberResults = rule.MaxNumberResults.Value;
        //
        //                 await InsertDiagnosticSqlIntoDetails(rule, newRuleExecutionLog, connectionString, databaseEnvironment.UserParams, existCatalog.Name, maxNumberResults);
        //             }
        //         }
        //     }
        //     catch(Exception ex)
        //     {
        //         newRuleExecutionLog.Result = -1;
        //         newRuleExecutionLog.Response = ex.Message;
        //         await _ruleExecutionLogCommands.UpdateAsync(newRuleExecutionLog);
        //     }
        //
        //     return testResult;
        // }
        //
        // public async Task<RuleTestResult> ExecuteRuleAsync(RuleBO rule, string connectionString, List<UserParamBO> userParams, int? timeout)
        // {
        //     var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        //     RuleTestResult testResult;
        //
        // }
        //
        // public async Task<TableResult> ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(int ruleLogId, DatabaseEnvironmentBO databaseEnvironment)
        // {
        //     TableResult result = new TableResult();
        //
        //     var existLog = await _ruleExecutionLogQueries.GetAsync(ruleLogId);
        //     var connectionString = databaseEnvironment.GetConnectionString();
        //
        //     try
        //     {
        //         if (!connectionString.ToLower().Contains("timeout") && databaseEnvironment.TimeoutInMinutes == null)
        //             connectionString += " Connection Timeout = 60";
        //         else if (databaseEnvironment.TimeoutInMinutes != null)
        //             connectionString += " Connection Timeout = " + (databaseEnvironment.TimeoutInMinutes.Value * 60).ToString();
        //
        //         using (var conn = new SqlConnection(connectionString))
        //         {
        //             await conn.OpenAsync();
        //
        //             using (var cmd = new SqlCommand(existLog.DiagnosticSql, conn))
        //             {
        //                 if (databaseEnvironment.TimeoutInMinutes != null)
        //                     cmd.CommandTimeout = (databaseEnvironment.TimeoutInMinutes.Value * 60);
        //
        //                 AddParameters(existLog.DiagnosticSql, cmd, databaseEnvironment.UserParams);
        //                 var getReader = await cmd.ExecuteReaderAsync();
        //                 DataTable dt = new DataTable();
        //                 dt.Load(getReader);
        //
        //                 result.Columns = dt.Columns.Count;
        //                 foreach (var dataColumn in dt.Columns)
        //                 {
        //                     result.ColumnsName.Add(dataColumn.ToString());
        //                 }
        //
        //                 var informationAsList = (from x in dt.AsEnumerable() select x).ToList();
        //                 result.Information = Utils.Serialize(result.ColumnsName, informationAsList);
        //                 result.Rows = result.Information.Count;
        //             }
        //         }
        //     }
        //     catch(Exception ex)
        //     {
        //         result.MessageError = ex.Message;
        //     }
        //
        //     return result;
        // }
        //
        // private async Task InsertDiagnosticSqlIntoDetails(RuleBO rule, RuleExecutionLog ruleExecutionLog, string connectionString, List<UserParamBO> sqlParams, string tableName, int maxNumberResults)
        // {
        //     string sqlToRun = Utils.GenerateSqlWithTop(rule.DiagnosticSql, maxNumberResults.ToString());
        //     string columnsSchema = string.Empty;
        //     var listColumnsFromDestination = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(tableName, "destination");
        //
        //     using (var conn = new SqlConnection(connectionString))
        //     {
        //         await conn.OpenAsync();
        //         using (var cmd = new SqlCommand(sqlToRun, conn))
        //         {
        //             AddParameters(sqlToRun,cmd,sqlParams);
        //             var reader = await cmd.ExecuteReaderAsync();
        //
        //             Dictionary<string, string> listColumns = new Dictionary<string, string>();
        //             listColumnsFromDestination.ForEach(rec => listColumns.Add(rec.Name, rec.Type));
        //
        //             DataTable tableToInsert = Utils.GetTableForSqlBulk(ruleExecutionLog.Id, reader, listColumns, out columnsSchema);
        //             if (tableToInsert != null && tableToInsert.Rows.Count > 0)
        //             {
        //                 if (ruleExecutionLog != null)
        //                 {
        //                     ruleExecutionLog.DetailsSchema = columnsSchema;
        //                     await _ruleExecutionLogCommands.UpdateAsync(ruleExecutionLog);
        //                 }
        //                 await _edFiRuleExecutionLogDetailCommands.ExecuteSqlBulkCopy(tableToInsert, $"[destination].[{tableName}]");
        //             }
        //         }
        //     }
        // }
        //
        // private void AddParameters(string sql, SqlCommand sqlCommand, List<UserParamBO> parameters)
        // {
        //     if (parameters != null)
        //     {
        //         parameters.ForEach(m =>
        //         {
        //             if (sql.Contains("@" + m.Name))
        //                 sqlCommand.Parameters.AddWithValue("@" + m.Name, m.Value);
        //         });
        //     }
        // }
        // }
    }
}
