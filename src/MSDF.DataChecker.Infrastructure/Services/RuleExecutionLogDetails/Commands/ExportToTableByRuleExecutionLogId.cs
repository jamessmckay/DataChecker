// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RuleExecutionLogDetails.Commands
{
    public class ExportToTableByRuleExecutionLogId
    {
        public class Command : IRequest<Result<RuleExecutionLogDetailExportToTableResource>>
        {
            public Command(int id) => Id = id;

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Command, Result<RuleExecutionLogDetailExportToTableResource>>
        {
            private readonly LegacyDatabaseContext _db;

            public Handler(LegacyDatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result<RuleExecutionLogDetailExportToTableResource>> Handle(Command request,
                CancellationToken cancellationToken)
            {
                var result = new RuleExecutionLogDetailExportToTableResource();

                var ruleExecutionLog = await _db.RuleExecutionLogs
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (ruleExecutionLog == null)
                {
                    return Result<RuleExecutionLogDetailExportToTableResource>.Fail(
                        $"No rule execution log detail for id '{request.Id}'");
                }

                if (!string.IsNullOrEmpty(ruleExecutionLog.DetailsTableName))
                {
                    result.TableName = ruleExecutionLog.DetailsTableName;

                    result.AlreadyExist = await ExistExportTableFromRuleExecutionLogAsync(
                        ruleExecutionLog.DetailsTableName, "destination");
                }

                if (!result.AlreadyExist)
                {
                    var ruleFromLog = await _db.Rules
                        .SingleOrDefaultAsync(rec => rec.Id == ruleExecutionLog.RuleId, cancellationToken);

                    string ruleName = ruleFromLog.Name;

                    ruleName = Regex.Replace(ruleName, @"[^\w\.@-]", "_", RegexOptions.None, TimeSpan.FromSeconds(1.5));
                    string tableName = $"T_{request.Id}-{ruleName}";

                    if (tableName.Length > 128)
                    {
                        tableName = tableName.Substring(0, 128);
                    }

                    Dictionary<string, string> columns = new Dictionary<string, string>();
                    columns = JsonSerializer.Deserialize<Dictionary<string, string>>(ruleExecutionLog.DetailsSchema);

                    List<string> sqlColumns = new List<string>();

                    foreach (var column in columns)
                    {
                        if (column.Value == "string")
                        {
                            sqlColumns.Add(string.Format($"[{column.Key}] [nvarchar](max) NULL"));
                        }
                        else
                        {
                            sqlColumns.Add(string.Format($"[{column.Key}] [datetime2](7) NULL"));
                        }
                    }

                    string sqlCreate = $"CREATE TABLE [destination].[{tableName}]({string.Join(",", sqlColumns)}) ";
                    await ExecuteSqlAsync(sqlCreate);

                    result.TableName = tableName;
                    result.Created = await ExistExportTableFromRuleExecutionLogAsync(tableName, "destination");

                    // TODO This needs sorting out
                    // if (result.Created)
                    // {
                    //     var ruleExecutionLogInfo = await GetByRuleExecutionLogIdAsync(id);
                    //
                    //     DataTable infoToInsert = Utils.GetTableForSqlBulk(ruleExecutionLogInfo.Rows, columns);
                    //     await _commandRuleExecutionLogDetail.ExecuteSqlBulkCopy(infoToInsert, $"[destination].[{tableName}]");
                    //
                    //     ruleExecutionLog.DetailsTableName = tableName;
                    //     await _commandRuleExecutionLog.UpdateAsync(ruleExecutionLog);
                    // }
                }

                return Result<RuleExecutionLogDetailExportToTableResource>.Success(result);

                async Task<bool> ExistExportTableFromRuleExecutionLogAsync(string tableName, string tableSchema)
                {
                    string connectionString = _db.Database.GetDbConnection().ConnectionString;

                    await using SqlConnection destinationConnection = new SqlConnection(connectionString);

                    await destinationConnection.OpenAsync(cancellationToken);

                    string sql = string.Format(
                        "SELECT COUNT(*) " +
                        "FROM INFORMATION_SCHEMA.TABLES " +
                        "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema ");

                    await using var sqlCommand = new SqlCommand(sql, destinationConnection);

                    sqlCommand.Parameters.AddWithValue("@tablename", tableName);
                    sqlCommand.Parameters.AddWithValue("@tableschema", tableSchema);

                    int result = (int) await sqlCommand.ExecuteScalarAsync(cancellationToken);

                    return result > 0;
                }

                async Task ExecuteSqlAsync(string sqlScript)
                {
                    string connectionString = _db.Database.GetDbConnection().ConnectionString;

                    await using SqlConnection destinationConnection = new SqlConnection(connectionString);

                    await destinationConnection.OpenAsync(cancellationToken);

                    await using var sqlCommand = new SqlCommand(sqlScript, destinationConnection);

                    await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }
    }
}
