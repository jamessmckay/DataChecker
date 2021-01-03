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
    public class GetById
    {
        public class Query : IRequest<Result<CollectionCategoryResource>>
        {
            public Query(Guid id) => Id = id;

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<CollectionCategoryResource>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            // TODO Address n+1
            public async Task<Result<CollectionCategoryResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var container = await _db.Containers
                    .Include(m => m.ChildContainers)
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (container == null)
                {
                    return Result<CollectionCategoryResource>.Fail($"Unable to find container for id '{request.Id}'");
                }

                var collectionCategory = new CollectionCategoryResource
                {
                    Id = container.Id,
                    Name = container.Name,
                    Description = container.Description,
                    EnvironmentType = container.EnvironmentType,
                    ChildContainers = new List<CollectionCategoryResource>(),
                    Rules = await GetWithLogsByContainerIdAsync(container.Id)
                };

                collectionCategory.ValidRules = collectionCategory.Rules.Count(m => m.LastStatus == 2);

                collectionCategory.LastStatus = collectionCategory.Rules.Any(m => m.LastStatus == 2)
                    ? 2
                    : 1;

                collectionCategory.ContainerTypeId = container.ContainerTypeId;
                collectionCategory.Tags = await GetByContainerIdAsync(collectionCategory.Id);
                collectionCategory.RuleDetailsDestinationId = container.RuleDetailsDestinationId;

                foreach (var itemContainer in container.ChildContainers)
                {
                    var category = new CollectionCategoryResource()
                    {
                        Id = itemContainer.Id,
                        Name = itemContainer.Name,
                        Rules = await GetWithLogsByContainerIdAsync(itemContainer.Id),
                        Tags = await GetByContainerIdAsync(itemContainer.Id)
                    };

                    category.ValidRules = category.Rules.Count(m => m.LastStatus == 2);

                    category.LastStatus = category.Rules.Any(m => m.LastStatus == 2)
                        ? 2
                        : 1;

                    if (collectionCategory.RuleDetailsDestinationId != null && category.Rules != null && category.Rules.Any())
                    {
                        category.Rules.ForEach(
                            rec => rec.CollectionRuleDetailsDestinationId = collectionCategory.RuleDetailsDestinationId);
                    }

                    collectionCategory.ChildContainers.Add(category);
                }

                return Result<CollectionCategoryResource>.Success(collectionCategory);

                async Task<List<RuleResource>> GetWithLogsByContainerIdAsync(Guid containerId)
                {
                    var rulesDatabase = await _db.Rules
                        .Where(rec => rec.ContainerId == containerId)
                        .ProjectTo<RuleResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                    foreach (var rule in rulesDatabase)
                    {
                        var latestLog = await _db.RuleExecutionLogs
                            .OrderByDescending(m => m.ExecutionDate)
                            .FirstOrDefaultAsync(x => x.RuleId == rule.Id, cancellationToken);

                        if (latestLog != null)
                        {
                            rule.Counter = latestLog.Result;
                            rule.LastExecution = latestLog.ExecutionDate;
                            rule.LastStatus = latestLog.StatusId;
                        }
                    }

                    return rulesDatabase;
                }

                async Task<List<TagResource>> GetByContainerIdAsync(Guid containerId)
                {
                    return await _db.TagEntities
                        .Where(rec => rec.ContainerId == containerId)
                        .Select(rec => rec.Tag)
                        .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);
                }
            }
        }
    }
}
