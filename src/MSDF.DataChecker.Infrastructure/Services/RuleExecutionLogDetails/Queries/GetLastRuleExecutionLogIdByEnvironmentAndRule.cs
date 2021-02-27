// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MSDF.DataChecker.Domain.Services.RuleExecutionLogDetails.Queries
{
    public class GetLastRuleExecutionLogIdByEnvironmentAndRule
    {
        public class Query : IRequest<Result<int?>>
        {
            public Query(Guid ruleId, Guid databaseEnvironmentId)
            {
                RuleId = ruleId;
                DatabaseEnvironmentId = databaseEnvironmentId;
            }

            public Guid RuleId { get; }

            public Guid DatabaseEnvironmentId { get; }
        }

        public class Handler : IRequestHandler<Query, Result<int?>>
        {
            private readonly LegacyDatabaseContext _db;

            public Handler(LegacyDatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result<int?>> Handle(Query request, CancellationToken cancellationToken)
            {
                var log = await _db.RuleExecutionLogs
                    .Where(m => m.RuleId == request.RuleId && m.DatabaseEnvironmentId == request.DatabaseEnvironmentId)
                    .OrderByDescending(m => m.ExecutionDate)
                    .FirstOrDefaultAsync(cancellationToken);

                return Result<int?>.Success(log?.Id);
            }
        }
    }
}
