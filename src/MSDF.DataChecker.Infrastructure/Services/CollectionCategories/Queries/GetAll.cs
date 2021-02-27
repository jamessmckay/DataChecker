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

namespace MSDF.DataChecker.Domain.Services.CollectionCategories.Queries
{
    public class GetAll
    {
        public class Query : IRequest<Result<List<CollectionCategoryResource>>> { }

        public class Handler : IRequestHandler<Query, Result<List<CollectionCategoryResource>>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            // TODO Address n+1
            public async Task<Result<List<CollectionCategoryResource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var serviceCollections = new List<CollectionCategoryResource>();

                var containers = await _db.Containers
                    .Where(m => m.ParentContainerId == null)
                    .Include(m => m.ChildContainers)
                    .ProjectTo<ContainerResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                if (!containers.Any())
                {
                    return Result<List<CollectionCategoryResource>>.Success(serviceCollections);
                }

                foreach (var container in containers)
                {
                    var currentCollection = new CollectionCategoryResource(container);

                    currentCollection.Tags = await _db.TagEntities
                        .Where(x => x.ContainerId == currentCollection.Id)
                        .Select(x => x.Tag)
                        .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                    foreach (var childContainer in currentCollection.ChildContainers)
                    {
                        childContainer.Rules = await GetWithLogsByContainerIdAsync(childContainer.Id);

                        childContainer.AmountRules = childContainer.Rules.Count;

                        childContainer.Tags = await _db.TagEntities
                            .Where(x => x.ContainerId == currentCollection.Id)
                            .Select(x => x.Tag)
                            .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                        if (container.RuleDetailsDestinationId != null && childContainer.Rules != null &&
                            childContainer.Rules.Any())
                        {
                            childContainer.Rules.ForEach(
                                rec => rec.CollectionRuleDetailsDestinationId = container.RuleDetailsDestinationId);
                        }
                    }

                    serviceCollections.Add(currentCollection);
                }

                return Result<List<CollectionCategoryResource>>.Success(serviceCollections);

                async Task<List<RuleResource>> GetWithLogsByContainerIdAsync(Guid containerId)
                {
                    var rulesDatabase = await _db.Rules
                        .Where(rec => rec.ContainerId == containerId)
                        .ProjectTo<RuleResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                    if (rulesDatabase.Any())
                    {
                        foreach (var ruleResource in rulesDatabase)
                        {
                            var latestLog = await _db.RuleExecutionLogs
                                .OrderByDescending(m => m.ExecutionDate)
                                .FirstOrDefaultAsync(m => m.RuleId == ruleResource.Id, cancellationToken);

                            if (latestLog == null)
                            {
                                continue;
                            }

                            ruleResource.Counter = latestLog.Result;
                            ruleResource.LastExecution = latestLog.ExecutionDate;
                            ruleResource.LastStatus = latestLog.StatusId;
                        }
                    }

                    return rulesDatabase;
                }
            }
        }
    }
}
