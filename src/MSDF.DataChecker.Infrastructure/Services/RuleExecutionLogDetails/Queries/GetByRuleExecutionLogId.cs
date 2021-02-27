// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RuleExecutionLogDetails.Queries
{
    public class GetByRuleExecutionLogId
    {
        public class Query : IRequest<Result<RuleExecutionLogDetailResource>>
        {
            public Query(int id) => Id = id;

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<RuleExecutionLogDetailResource>>
        {
            private readonly LegacyDatabaseContext _db;

            public Handler(LegacyDatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result<RuleExecutionLogDetailResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = new RuleExecutionLogDetailResource();

                var ruleExecutionLog = await _db.RuleExecutionLogs
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (ruleExecutionLog == null)
                {
                    return Result<RuleExecutionLogDetailResource>.Fail($"No execution log details for id '{request.Id}'");
                }

                if (ruleExecutionLog.RuleDetailsDestinationId != null)
                {
                    await AddDetailsForDestination();
                }

                return Result<RuleExecutionLogDetailResource>.Success(result);

                async Task AddDetailsForDestination()
                {
                    var existRule = await _db.Rules
                        .SingleOrDefaultAsync(x => x.Id == ruleExecutionLog.RuleId, cancellationToken);

                    var existDatabaseEnvironment =
                        await _db.DatabaseEnvironments
                            .Include(x => x.UserParams)
                            .SingleOrDefaultAsync(x => x.Id == ruleExecutionLog.DatabaseEnvironmentId, cancellationToken);

                    result.RuleName = existRule.Name;
                    result.EnvironmentName = existDatabaseEnvironment.Name;
                    result.ExecutionDateTime = ruleExecutionLog.ExecutionDate.ToLocalTime().ToString("MM/dd/yyyy HH:mm");

                    var catalog = await _db.Catalogs
                        .SingleOrDefaultAsync(x => x.Id == ruleExecutionLog.RuleDetailsDestinationId.Value, cancellationToken);

                    if (catalog != null)
                    {
                        await AddDetailsFor(catalog);
                    }

                    async Task AddDetailsFor(LegacyCatalog catalog)
                    {
                        result.DestinationTable = catalog.Name;
                        result.RuleDiagnosticSql = ruleExecutionLog.DiagnosticSql;

                        Dictionary<string, string> columns = new Dictionary<string, string>();

                        if (!string.IsNullOrEmpty(ruleExecutionLog.DetailsSchema))
                        {
                            columns = JsonSerializer.Deserialize<Dictionary<string, string>>(ruleExecutionLog.DetailsSchema);
                        }

                        var dataTable = await GetByRuleExecutionLogIdAsync(request.Id, catalog.Name);

                        if (dataTable != null)
                        {
                            List<string> columnsToIgnore = new List<string>()
                            {
                                "id",
                                "otherdetails",
                                "ruleexecutionlogid"
                            };

                            List<string> columnsToExport = new List<string>();

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                string columnName = column.ColumnName.ToLower();

                                if (!columnsToIgnore.Contains(columnName) &&
                                    (columns.Count == 0 || columns.ContainsKey(columnName)))
                                {
                                    columnsToExport.Add(columnName);
                                }
                            }

                            List<Dictionary<string, string>> rowsToExport = new List<Dictionary<string, string>>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                Dictionary<string, string> newRow = new Dictionary<string, string>();

                                foreach (var column in columnsToExport)
                                {
                                    newRow.Add(column, row[column].ToString());
                                }

                                string otherDetails = row.Field<string>("otherdetails").ToString();

                                if (!string.IsNullOrEmpty(otherDetails))
                                {
                                    Dictionary<string, string> jsonValues =
                                        JsonSerializer.Deserialize<Dictionary<string, string>>(otherDetails);

                                    foreach (var element in jsonValues)
                                    {
                                        if (columnsToExport.Contains(element.Key) || columns.ContainsKey(element.Key))
                                        {
                                            newRow.Add(element.Key, element.Value);
                                        }
                                    }
                                }

                                rowsToExport.Add(newRow);
                            }

                            if (columns.Count > 0)
                            {
                                columnsToExport = columns.Select(rec => rec.Key).ToList();
                            }

                            result.RuleExecutionLogId = request.Id;
                            result.Columns = columnsToExport;
                            result.Rows = rowsToExport;
                        }

                        async Task<DataTable> GetByRuleExecutionLogIdAsync(int id, string tableName)
                        {
                            string connectionString = _db.Database.GetDbConnection().ConnectionString;

                            await using SqlConnection destinationConnection = new SqlConnection(connectionString);

                            await destinationConnection.OpenAsync(cancellationToken);

                            string sql = string.Format(
                                "SELECT * " +
                                "FROM destination.{0} " +
                                "WHERE RuleExecutionLogId = @Id " +
                                "ORDER BY Id", tableName);

                            await using var sqlCommand = new SqlCommand(sql, destinationConnection);
                            sqlCommand.Parameters.AddWithValue("@Id", id);
                            var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

                            if (!reader.HasRows)
                            {
                                return null;
                            }

                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            return dt;
                        }
                    }
                }
            }
        }
    }
}
