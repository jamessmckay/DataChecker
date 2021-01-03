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
using MSDF.DataChecker.Domain.Providers;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Commands
{
    public class Add
    {
        public class Command : IRequest<Result<Guid>>
        {
            public Command(DatabaseEnvironmentResource databaseEnvironmentResource) => Resource = databaseEnvironmentResource;

            public DatabaseEnvironmentResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly IDatabaseMapProvider _databaseMapProvider;
            private readonly DatabaseContext _db;
            private readonly IEncryptionProvider _encryptionProvider;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper,
                IDatabaseMapProvider databaseMapProvider,
                IEncryptionProvider encryptionProvider)
            {
                _db = db;
                _mapper = mapper;
                _databaseMapProvider = databaseMapProvider;
                _encryptionProvider = encryptionProvider;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                request.Resource.MapTables = await _databaseMapProvider.GetJson(request.Resource.GetConnectionString());

                if (request.Resource.SecurityIntegrated == null || !request.Resource.SecurityIntegrated.Value)
                {
                    request.Resource.Password = await _encryptionProvider.EncryptStringAsync(request.Resource.Password);
                }

                var created = DateTime.Now;

                var entity = await _db.DatabaseEnvironments
                    .SingleOrDefaultAsync(x => x.Id == request.Resource.Id, cancellationToken);

                if (entity == null)
                {
                    entity = _mapper.Map<DatabaseEnvironmentResource, DatabaseEnvironment>(request.Resource);
                    entity.CreatedDate = created;
                    await _db.DatabaseEnvironments.AddAsync(entity, cancellationToken);
                }
                else
                {
                    entity = _mapper.Map(request.Resource, entity);
                    _db.DatabaseEnvironments.Update(entity);
                }

                await _db.SaveChangesAsync(cancellationToken);

                var result = Result<Guid>.Success(entity.Id);

                result.IsUpdated = entity.CreatedDate != created;

                return result;
            }
        }
    }
}
