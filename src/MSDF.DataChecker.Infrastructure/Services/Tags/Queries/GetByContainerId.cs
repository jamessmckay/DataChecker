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
    public class GetByContainerId
    {
        public class Query : IRequest<Result<List<TagResource>>>
        {
            public Query(Guid id) => Id = id;

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<List<TagResource>>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<TagResource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var resources = await _db.TagEntities
                    .Where(x => x.ContainerId == request.Id)
                    .Select(x => x.Tag)
                    .OrderBy(x => x.Name)
                    .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                await SetTagIsUsed();

                return Result<List<TagResource>>.Success(resources);

                async Task SetTagIsUsed()
                {
                    // TODO: This logic needs to be reworked
                    if (resources.Any())
                    {
                        var tagIds = resources
                            .Select(x => x.Id)
                            .ToList();

                        var entities = await _db.TagEntities
                            .Where(x => tagIds.Contains(x.TagId))
                            .ToListAsync(cancellationToken);

                        foreach (var resource in resources.Where(x => tagIds.Contains(x.Id)))
                        {
                            resource.IsUsed = entities.Any(x => x.TagId == resource.Id);
                        }
                    }
                }
            }
        }
    }
}
