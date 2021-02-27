// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RulesExecution.Commands
{
    public class ExecuteRuleByEnvironmentId
    {
        public class Command : IRequest<Result<RuleTestResultResource>>
        {
            public Command(Guid ruleId, Guid databaseEnvironmentId)
            {
                RuleId = ruleId;
                DatabaseEnvironmentId = databaseEnvironmentId;
            }

            public Guid RuleId { get; }

            public Guid DatabaseEnvironmentId { get; }
        }

        public class Handler : IRequestHandler<Command, Result<RuleTestResultResource>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;
            private readonly IRuleRunner _ruleRunner;

            public Handler(LegacyDatabaseContext db, IMapper mapper, IRuleRunner ruleRunner)
            {
                _db = db;
                _mapper = mapper;
                _ruleRunner = ruleRunner;
            }

            public async Task<Result<RuleTestResultResource>> Handle(Command request, CancellationToken cancellationToken)
            {
                var databaseEnvironment = await _db.DatabaseEnvironments
                    .SingleOrDefaultAsync(x => x.Id == request.DatabaseEnvironmentId, cancellationToken);

                if (databaseEnvironment == null)
                {
                    return Result<RuleTestResultResource>.Fail(
                        $"Database environment does not exist for '{request.DatabaseEnvironmentId}");
                }

                var rule = await _db.Rules
                    .SingleOrDefaultAsync(x => x.Id == request.RuleId, cancellationToken);

                if (rule == null)
                {
                    return Result<RuleTestResultResource>.Fail($"Rule does not exist for '{rule.Id}");
                }

                var executionLogs = await _db.RuleExecutionLogs
                    .Where(m => m.RuleId == rule.Id)
                    .OrderByDescending(m => m.ExecutionDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                int? ruleDetailsDestinationId = null;

                var sw = Stopwatch.StartNew();
                var testResult = await _ruleRunner.ExecuteRuleAsync(rule, databaseEnvironment);

                var containerParent = await _db.Containers
                    .Where(m => m.Id == rule.Id)
                    .Include(m => m.ChildContainers)
                    .SingleOrDefaultAsync(cancellationToken);

                if (containerParent.ParentContainerId != null)
                {
                    var collectionParent = await _db.Containers
                        .Where(m => m.Id == containerParent.ParentContainerId.Value)
                        .Include(m => m.ChildContainers)
                        .SingleOrDefaultAsync(cancellationToken);

                    ruleDetailsDestinationId = collectionParent.RuleDetailsDestinationId;
                }

                var ruleExecutionLog = new RuleExecutionLog
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

                testResult.LastExecuted = executionLogs.FirstOrDefault()?.ExecutionDate;
                ruleExecutionLog.ExecutionTimeMs = sw.ElapsedMilliseconds;
                ruleExecutionLog.Result = testResult.Result;

                var newRuleExecutionLog = await _db.RuleExecutionLogs.AddAsync(ruleExecutionLog, cancellationToken);

                testResult.TestResults = await _db.RuleExecutionLogs
                    .Where(m => m.RuleId == rule.Id && m.DatabaseEnvironmentId == databaseEnvironment.Id)
                    .OrderByDescending(m => m.ExecutionDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                if (!testResult.Evaluation)
                {
                    testResult.DiagnosticSql = rule.DiagnosticSql;
                }

                // TODO: This logic seem odd here. Note the insert is really odd, and should be re-evaluated.
                // try
                // {
                //     //Validate if the rule is going to any queue table
                //     if (ruleDetailsDestinationId != null && ruleDetailsDestinationId.Value > 0)
                //     {
                //         var existCatalog = await _db.Catalogs
                //             .SingleOrDefaultAsync(x => x.Id == ruleDetailsDestinationId.Value, cancellationToken);
                //
                //         if (existCatalog != null)
                //         {
                //             int maxNumberResults = databaseEnvironment.MaxNumberResults.Value;
                //
                //             if (rule.MaxNumberResults != null)
                //                 maxNumberResults = rule.MaxNumberResults.Value;
                //
                //             await InsertDiagnosticSqlIntoDetails(rule, newRuleExecutionLog, connectionString, databaseEnvironment.UserParams, existCatalog.Name, maxNumberResults);
                //         }
                //     }
                // }
                // catch(Exception ex)
                // {
                //     newRuleExecutionLog.Result = -1;
                //     newRuleExecutionLog.Response = ex.Message;
                //     await _ruleExecutionLogCommands.UpdateAsync(newRuleExecutionLog);
                // }

                return Result<RuleTestResultResource>.Success(_mapper.Map<RuleTestResult, RuleTestResultResource>(testResult));
            }
        }
    }
}
