// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Providers;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RuleExecutionLogDetails.Queries
{
    public class GetExecutionDiagnosticSqlByLogId
    {
        public class Query : IRequest<Result<RuleExecutionLogDetailResource>>
        {
            public Query(int id) => Id = id;

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<RuleExecutionLogDetailResource>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IDatabaseEnvironmentConnectionStringProvider _environmentConnectionStringProvider;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper,
                IDatabaseEnvironmentConnectionStringProvider environmentConnectionStringProvider)
            {
                _db = db;
                _mapper = mapper;
                _environmentConnectionStringProvider = environmentConnectionStringProvider;
            }

            public async Task<Result<RuleExecutionLogDetailResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ruleExecutionLog = await _db.RuleExecutionLogs.SingleOrDefaultAsync(
                    executionLog => executionLog.Id == request.Id, cancellationToken);

                if (ruleExecutionLog == null)
                {
                    return Result<RuleExecutionLogDetailResource>.Fail($"Unable to find a rule execution log for '{request.Id}'");
                }

                var result = new RuleExecutionLogDetailResource();

                var env = await _db.DatabaseEnvironments
                    .Include(x => x.UserParams)
                    .SingleOrDefaultAsync(x => x.Id == ruleExecutionLog.DatabaseEnvironmentId, cancellationToken);

                string connectionString = await _environmentConnectionStringProvider.GetConnectionString(env);

                if (!connectionString.ToLower().Contains("timeout") && env.TimeoutInMinutes == null)
                {
                    connectionString += " Connection Timeout = 60";
                }
                else if (env.TimeoutInMinutes != null)
                {
                    connectionString += " Connection Timeout = " + (env.TimeoutInMinutes.Value * 60).ToString();
                }

                await using var conn = new SqlConnection(connectionString);

                await conn.OpenAsync(cancellationToken);

                await using var cmd = new SqlCommand(ruleExecutionLog.DiagnosticSql, conn);

                if (env.TimeoutInMinutes != null)
                {
                    cmd.CommandTimeout = env.TimeoutInMinutes.Value * 60;
                }

                result.RuleDiagnosticSql = ruleExecutionLog.DiagnosticSql;

                var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                if (reader.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    if (dt != null)
                    {
                        List<string> columnsToExport = new List<string>();

                        foreach (DataColumn column in dt.Columns)
                        {
                            string columnName = column.ColumnName.ToLower();
                            columnsToExport.Add(columnName);
                        }

                        List<Dictionary<string, string>> rowsToExport = new List<Dictionary<string, string>>();

                        foreach (DataRow row in dt.Rows)
                        {
                            Dictionary<string, string> newRow = new Dictionary<string, string>();

                            foreach (var column in columnsToExport)
                            {
                                newRow.Add(column, row[column].ToString());
                            }

                            rowsToExport.Add(newRow);
                        }

                        result.RuleExecutionLogId = request.Id;
                        result.Columns = columnsToExport;
                        result.Rows = rowsToExport;
                    }
                }

                return Result<RuleExecutionLogDetailResource>.Success(result);
            }
        }
    }
}
