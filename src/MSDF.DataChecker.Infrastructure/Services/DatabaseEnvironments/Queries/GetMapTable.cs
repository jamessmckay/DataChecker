// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MSDF.DataChecker.Domain.Providers;

namespace MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Queries
{
    public class GetMapTable
    {
        public class Query : IRequest<Result<IDictionary<string, List<string>>>>
        {
            public Query(Guid id) => Id = id;

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<IDictionary<string, List<string>>>>
        {
            private readonly IDatabaseEnvironmentConnectionStringProvider _connectionStringProvider;
            private readonly IDatabaseMapProvider _databaseMapProvider;
            private readonly LegacyDatabaseContext _db;

            public Handler(LegacyDatabaseContext db, IDatabaseMapProvider databaseMapProvider,
                IDatabaseEnvironmentConnectionStringProvider connectionStringProvider)
            {
                _db = db;
                _databaseMapProvider = databaseMapProvider;
                _connectionStringProvider = connectionStringProvider;
            }

            public async Task<Result<IDictionary<string, List<string>>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var entity = _db.DatabaseEnvironments
                    .SingleOrDefault(x => x.Id == request.Id);

                if (entity == null)
                {
                    Result<IDictionary<string, object>>.Fail($"DatabaseEnvironment does not exists for  '{request.Id}'");
                }

                IDictionary<string, List<string>> map;

                if (string.IsNullOrEmpty(entity.MapTables))
                {
                    string connectionString = await _connectionStringProvider.GetConnectionString(entity);
                    map = await _databaseMapProvider.Get(connectionString);
                    entity.MapTables = JsonSerializer.Serialize(map);
                    _db.DatabaseEnvironments.Update(entity);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    map = JsonSerializer.Deserialize<IDictionary<string, List<string>>>(entity.MapTables);
                }

                return Result<IDictionary<string, List<string>>>.Success(map);
            }
        }
    }
}
