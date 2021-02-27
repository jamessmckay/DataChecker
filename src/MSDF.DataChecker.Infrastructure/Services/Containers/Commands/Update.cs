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
    public class Update
    {
        public class Command : IRequest<Result<bool>>
        {
            public Command(ContainerResource resource) => Resource = resource;

            public ContainerResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.Containers
                    .SingleOrDefaultAsync(x => x.Id == request.Resource.Id, cancellationToken);

                if (entity == null)
                {
                    return Result<bool>.Fail($"Container not found for '{request.Resource.Id}'");
                }

                var updated = DateTime.UtcNow;

                entity = _mapper.Map(request.Resource, entity);

                _db.Containers.Update(entity);

                await CreateUpdateTags(request.Resource, entity);

                await _db.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);

                async Task CreateUpdateTags(ContainerResource resource, Container container)
                {
                    var tags = await _db.TagEntities
                        .Where(rec => rec.ContainerId == container.Id)
                        .Select(rec => rec.Tag)
                        .ToListAsync(cancellationToken);

                    if (resource.Tags.Any())
                    {
                        foreach (var tag in tags)
                        {
                            var existTag = resource.Tags.FirstOrDefault(rec => rec.Id == tag.Id);

                            if (existTag == null)
                            {
                                await DeleteTagFromEntityAsync(tag.Id, container.Id);
                            }
                        }

                        foreach (var tagResource in resource.Tags)
                        {
                            var existTag = tags.FirstOrDefault(rec => rec.Id == tagResource.Id);

                            if (existTag == null)
                            {
                                if (tagResource.Id == -1)
                                {
                                    var tag = _mapper.Map<Tag>(tagResource);
                                    tag.Created = updated;
                                    tag.Updated = updated;

                                    await _db.Tags.AddAsync(tag, cancellationToken);

                                    tagResource.Id = tag.Id;
                                }

                                await _db.TagEntities.AddAsync(
                                    new TagEntity
                                    {
                                        ContainerId = container.Id,
                                        TagId = tagResource.Id
                                    }, cancellationToken);
                            }
                        }
                    }
                    else if (tags != null && tags.Count > 0)
                    {
                        foreach (var tag in tags)
                        {
                            await DeleteTagFromEntityAsync(tag.Id, container.Id);
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
