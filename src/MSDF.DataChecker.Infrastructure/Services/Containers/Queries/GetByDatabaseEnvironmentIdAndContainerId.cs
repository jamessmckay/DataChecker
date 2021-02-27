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

namespace MSDF.DataChecker.Domain.Services.Containers.Queries
{
    public class GetByDatabaseEnvironmentIdAndContainerId
    {
        public class Query : IRequest<Result<CollectionCategoryResource>>
        {
            public Query(Guid databaseEnvironmentId, Guid containerId)
            {
                DatabaseEnvironmentId = databaseEnvironmentId;
                ContainerId = containerId;
            }

            public Guid DatabaseEnvironmentId { get; }

            public Guid ContainerId { get; }
        }

        // TODO Address n+1
        public class Handler : IRequestHandler<Query, Result<CollectionCategoryResource>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<CollectionCategoryResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var container = await _db.Containers
                    .Include(m => m.ChildContainers)
                    .SingleOrDefaultAsync(x => x.Id == request.ContainerId, cancellationToken);

                if (container == null)
                {
                    return Result<CollectionCategoryResource>.Fail($"Unable to load container for id {request.ContainerId}");
                }

                var collectionCategory = new CollectionCategoryResource
                {
                    Id = container.Id,
                    Name = container.Name,
                    EnvironmentType = container.EnvironmentType,
                    Description = container.Description,
                    ContainerTypeId = container.ContainerTypeId,
                    RuleDetailsDestinationId = container.RuleDetailsDestinationId,
                    ChildContainers = new List<CollectionCategoryResource>(),
                    Tags = await GetByContainerIdAsync(container.Id)
                };

                foreach (var itemContainer in container.ChildContainers)
                {
                    var category = new CollectionCategoryResource
                    {
                        Id = itemContainer.Id,
                        Name = itemContainer.Name,
                        Rules = new List<RuleResource>(),
                        ContainerTypeId = container.ContainerTypeId,

                        //CreatedByUserId = container.CreatedByUserId,
                        LastStatus = 0,
                        Tags = await GetByContainerIdAsync(itemContainer.Id)
                    };

                    category.Rules = await GetWithLogsByDatabaseEnvironmentIdAndContainerIdAsync(
                        request.DatabaseEnvironmentId, itemContainer.Id);

                    category.ValidRules = category.Rules.Count(m => m.LastStatus == 2);

                    if (category.Rules.Any())
                    {
                        if (category.Rules.Count(rec => rec.LastStatus == 3) > 0)
                        {
                            category.LastStatus = 3;
                        }
                        else if (category.Rules.Count(rec => rec.LastStatus == 2) > 0)
                        {
                            category.LastStatus = 2;
                        }
                        else if (category.Rules.Count(rec => rec.LastStatus == 1) > 0)
                        {
                            category.LastStatus = 1;
                        }
                    }

                    if (collectionCategory.RuleDetailsDestinationId != null && category.Rules != null && category.Rules.Any())
                    {
                        category.Rules.ForEach(
                            rec => rec.CollectionRuleDetailsDestinationId = collectionCategory.RuleDetailsDestinationId);
                    }

                    collectionCategory.ChildContainers.Add(category);
                }

                return Result<CollectionCategoryResource>.Success(collectionCategory);

                async Task<List<TagResource>> GetByContainerIdAsync(Guid containerId)
                {
                    return await _db.TagEntities
                        .Where(rec => rec.ContainerId == containerId)
                        .Select(rec => rec.Tag)
                        .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);
                }

                async Task<List<RuleResource>> GetWithLogsByDatabaseEnvironmentIdAndContainerIdAsync(Guid databaseEnvironmentId,
                    Guid containerId)
                {
                    var rules = await _db.Rules
                        .Where(rec => rec.ContainerId == containerId)
                        .ProjectTo<RuleResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                    foreach (var rule in rules)
                    {
                        var latestLogRecord = await _db.RuleExecutionLogs
                            .OrderByDescending(m => m.ExecutionDate)
                            .FirstOrDefaultAsync(
                                m => m.RuleId == rule.Id && m.DatabaseEnvironmentId == databaseEnvironmentId, cancellationToken);

                        if (latestLogRecord == null)
                        {
                            continue;
                        }

                        rule.Counter = latestLogRecord.Result;
                        rule.LastExecution = latestLogRecord.ExecutionDate;
                        rule.LastStatus = latestLogRecord.StatusId;
                    }

                    return rules;
                }
            }
        }
    }
}
