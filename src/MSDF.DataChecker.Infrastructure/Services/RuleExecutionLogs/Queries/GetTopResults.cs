// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RuleExecutionLogs.Queries
{
    //TODO: This Should belong with RuleExecutionLogServices
    public class GetTopResults
    {
        public class Query : IRequest<Result<List<RuleTestResultResource>>>
        {
            public Query(Guid ruleId, Guid databaseEnvironmentId)
            {
                RuleId = ruleId;
                DatabaseEnvironmentId = databaseEnvironmentId;
            }

            public Guid RuleId { get; }

            public Guid DatabaseEnvironmentId { get; }
        }

        public class Handler : IRequestHandler<Query, Result<List<RuleTestResultResource>>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<RuleTestResultResource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var results = await _db.RuleExecutionLogs
                    .Where(m => m.RuleId == request.RuleId && m.DatabaseEnvironmentId == request.DatabaseEnvironmentId)
                    .OrderByDescending(m => m.ExecutionDate)
                    .Take(5)
                    .ProjectTo<RuleTestResultResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<RuleTestResultResource>>.Success(results);
            }
        }
    }
}
