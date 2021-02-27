// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Providers;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Commands
{
    public class Update
    {
        public class Command : IRequest<Result<DatabaseEnvironmentResource>>
        {
            public Command(DatabaseEnvironmentResource resource) => Resource = resource;

            public DatabaseEnvironmentResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<DatabaseEnvironmentResource>>
        {
            private readonly IDatabaseMapProvider _databaseMapProvider;
            private readonly LegacyDatabaseContext _db;
            private readonly IEncryptionProvider _encryptionProvider;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper, IEncryptionProvider encryptionProvider,
                IDatabaseMapProvider databaseMapProvider)
            {
                _db = db;
                _mapper = mapper;
                _encryptionProvider = encryptionProvider;
                _databaseMapProvider = databaseMapProvider;
            }

            public async Task<Result<DatabaseEnvironmentResource>>
                Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.DatabaseEnvironments
                    .SingleOrDefaultAsync(x => x.Id == request.Resource.Id, cancellationToken);

                if (entity == null)
                {
                    return Result<DatabaseEnvironmentResource>.Fail($"No DatabaseEnvironment for '{request.Resource.Id}'");
                }

                if (entity.SecurityIntegrated != null && entity.SecurityIntegrated.Value)
                {
                    entity.User = null;
                    entity.Password = null;
                }

                request.Resource.MapTables = await _databaseMapProvider.GetJson(request.Resource.GetConnectionString());

                if (request.Resource.SecurityIntegrated == null || !request.Resource.SecurityIntegrated.Value)
                {
                    request.Resource.Password = await _encryptionProvider.EncryptStringAsync(request.Resource.Password);
                }

                _mapper.Map(request.Resource, entity);

                _db.Update(entity);
                await _db.SaveChangesAsync(cancellationToken);

                var resource = _mapper.Map(entity, request.Resource);
                return Result<DatabaseEnvironmentResource>.Success(resource);
            }
        }
    }
}
