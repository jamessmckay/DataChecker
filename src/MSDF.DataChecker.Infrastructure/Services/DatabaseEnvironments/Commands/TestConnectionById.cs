// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Providers;

namespace MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Commands
{
    public class TestConnectionById
    {
        public class Command : IRequest<Result<bool>>
        {
            public Command(Guid id) => Id = id;

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly IDatabaseEnvironmentConnectionStringProvider _connectionStringProvider;
            private readonly DatabaseContext _db;

            public Handler(DatabaseContext db, IDatabaseEnvironmentConnectionStringProvider connectionStringProvider)
            {
                _db = db;
                _connectionStringProvider = connectionStringProvider;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.DatabaseEnvironments
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                {
                    Result<bool>.Fail($"DatabaseEnvironment not found for '{request.Id}'");
                }

                var connectionString = await _connectionStringProvider.GetConnectionString(entity);

                await using var conn = new SqlConnection(connectionString);

                await conn.OpenAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
        }
    }
}
