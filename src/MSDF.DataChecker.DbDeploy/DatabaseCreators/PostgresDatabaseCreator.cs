// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MSDF.DataChecker.DbDeploy.DatabaseCreators
{
    public class PostgresDatabaseCreator : IDatabaseCreator
    {
        private readonly ILogger<PostgresDatabaseCreator> _logger;
        private readonly string _connectionString;

        public PostgresDatabaseCreator(ILogger<PostgresDatabaseCreator> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DataCheckerStore");
        }

        public void EnsureDatabaseIsCreated()
        {
            EnsureDatabase.For.PostgresqlDatabase(_connectionString);
        }
    }
}
