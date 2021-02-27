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
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.UserParams.Commands
{
    public class UpdateUserParams
    {
        public class Command : IRequest<Result<List<UserParamResource>>>
        {
            public Command(Guid id, List<UserParamResource> resources)
            {
                Id = id;
                Resources = resources;
            }

            public Guid Id { get; }

            public List<UserParamResource> Resources { get; }
        }

        public class Handler : IRequestHandler<Command, Result<List<UserParamResource>>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<UserParamResource>>> Handle(Command request, CancellationToken cancellationToken)
            {
                var resources = request.Resources;

                var entities = await _db.UserParams
                    .Where(x => x.DatabaseEnvironmentId == request.Id)
                    .ToListAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    var resource = resources.FirstOrDefault(rec => rec.Id == entity.Id);

                    // if we have a resource then we update the entity
                    if (resource != null)
                    {
                        entity.Name = resource.Name;
                        entity.Value = resource.Value;
                        _db.UserParams.Update(entity);
                        resources.Remove(resource);
                    }
                    else
                    {
                        // entity is orphaned, so we removed it
                        _db.UserParams.Remove(entity);
                    }
                }

                // add any new resources
                foreach (var resource in resources)
                {
                    resource.DatabaseEnvironmentId = request.Id;
                    await _db.UserParams.AddAsync(_mapper.Map<UserParamResource, UserParam>(resource), cancellationToken);
                }

                await _db.SaveChangesAsync(cancellationToken);

                var results = await _db.UserParams
                    .Where(x => x.DatabaseEnvironmentId == request.Id)
                    .ProjectTo<UserParamResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<UserParamResource>>.Success(results);
            }
        }
    }
}
