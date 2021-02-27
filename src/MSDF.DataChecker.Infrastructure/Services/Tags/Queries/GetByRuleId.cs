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

namespace MSDF.DataChecker.Domain.Services.Tags.Queries
{
    public class GetByRuleId
    {
        public class Query : IRequest<Result<List<TagResource>>>
        {
            public Query(Guid id) => Id = id;

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<List<TagResource>>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<TagResource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var resources = await _db.TagEntities
                    .Where(rec => rec.RuleId == request.Id)
                    .Select(rec => rec.Tag)
                    .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<TagResource>>.Success(resources);
            }
        }
    }
}
