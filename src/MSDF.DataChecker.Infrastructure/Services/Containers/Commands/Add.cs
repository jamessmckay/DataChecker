// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Containers.Commands
{
    public class Add
    {
        public class Command : IRequest<Result<Guid>>
        {
            public Command(ContainerResource resource)
            {
                Resource = resource;
            }

            public ContainerResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.Containers.SingleOrDefaultAsync(x => x.Id == request.Resource.Id, cancellationToken);

                bool isUpdated = entity != null;

                entity = _mapper.Map(request.Resource, entity);

                if (isUpdated)
                {
                    _db.Containers.Update(entity);
                }
                else
                {
                    await _db.Containers.AddAsync(entity, cancellationToken);
                    await CreateUpdateTags(request.Resource, entity);
                }

                await _db.SaveChangesAsync(cancellationToken);

                var result = Result<Guid>.Success(entity.Id);
                result.IsUpdated = isUpdated;

                return result;

                async Task CreateUpdateTags(ContainerResource resource, Container entity)
                {
                    var listTags = await _db.TagEntities
                        .Where(x => x.ContainerId == entity.Id)
                        .Select(x => x.Tag)
                        .ToListAsync(cancellationToken);

                    if (resource.Tags.Any())
                    {
                        foreach (var tag in listTags)
                        {
                            var existTag = resource.Tags.SingleOrDefault(x => x.Id == tag.Id);

                            if (existTag == null)
                            {
                                await DeleteTagFromEntityAsync(tag.Id, entity.Id);
                            }
                        }

                        foreach (var resourceTag in resource.Tags)
                        {
                            var existTag = listTags.SingleOrDefault(x => x.Id == resourceTag.Id);

                            if (existTag == null)
                            {
                                var tag = _mapper.Map<Tag>(resourceTag);

                                if (resourceTag.Id == -1)
                                {
                                    await _db.Tags.AddAsync(tag, cancellationToken);

                                    resourceTag.Id = tag.Id;
                                }

                                await _db.TagEntities.AddAsync(
                                    new TagEntity
                                    {
                                        ContainerId = entity.Id,
                                        TagId = resourceTag.Id
                                    }, cancellationToken);
                            }
                        }
                    }
                    else if (listTags != null && listTags.Count > 0)
                    {
                        foreach (var tag in listTags)
                        {
                            await DeleteTagFromEntityAsync(tag.Id, entity.Id);
                        }
                    }

                    async Task DeleteTagFromEntityAsync(int id, Guid idEntity)
                    {
                        var entityToDelete = await _db.TagEntities
                            .SingleOrDefaultAsync(
                                x => x.TagId == id
                                     && (x.ContainerId == idEntity || x.RuleId == idEntity),
                                cancellationToken);

                        if (entityToDelete != null)
                        {
                            _db.TagEntities.Remove(entityToDelete);
                        }
                    }
                }
            }
        }
    }
}
