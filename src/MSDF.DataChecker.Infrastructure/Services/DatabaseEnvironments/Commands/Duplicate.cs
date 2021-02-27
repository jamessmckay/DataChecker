// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Commands
{
    public class Duplicate
    {
        public class Command : IRequest<Result<Guid>>
        {
            public Command(Guid id) => Id = id;

            public Guid Id { get; }
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
                var entity = await _db.DatabaseEnvironments
                    .Include(x => x.UserParams)
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                {
                    return Result<Guid>.Fail($"DatabaseEnvironment does not exists for '{request.Id}'");
                }

                var resource = _mapper.Map<DatabaseEnvironment, DatabaseEnvironmentResource>(entity);
                resource.Id = Guid.Empty;
                resource.Name = $"{entity.Name}_Dup";
                resource.MapTables = entity.MapTables;

                var newEntity = _mapper.Map<DatabaseEnvironmentResource, DatabaseEnvironment>(resource);

                await _db.DatabaseEnvironments.AddAsync(newEntity, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(entity.Id);
            }
        }
    }
}
