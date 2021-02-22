// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MSDF.DataChecker.DbDeploy.UpgradeEngineBuilderFactories
{
    public class PostgresUpgradeEngineFactory : IUpgradeEngineFactory
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;

        public PostgresUpgradeEngineFactory(ILogger<PostgresUpgradeEngineFactory> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DataCheckerStore");
        }

        public UpgradeEngine[] Create()
        {
            return new[]
            {
                DeployChanges.To
                    .PostgresqlDatabase(_connectionString)
                    .JournalToPostgresqlTable(DeployConventions.Postgres.DefaultSchema, DeployConventions.JournalTable)
                    .WithVariablesDisabled()
                    .WithScriptsEmbeddedInAssembly(
                        typeof(Program).Assembly,
                        s => s.StartsWith(
                            $"{DeployConventions.ScriptsDirectory}.{DeployConventions.PgSql}.{DeployConventions.StructureDirectory}",
                            StringComparison.InvariantCultureIgnoreCase))
                    .LogToAutodetectedLog()
                    .Build(),
                DeployChanges.To
                    .PostgresqlDatabase(_connectionString)
                    .JournalToPostgresqlTable(DeployConventions.Postgres.DefaultSchema, DeployConventions.JournalTable)
                    .WithVariablesDisabled()
                    .WithScriptsEmbeddedInAssembly(
                        typeof(Program).Assembly,
                        s => s.StartsWith(
                            $"{DeployConventions.ScriptsDirectory}.{DeployConventions.PgSql}.{DeployConventions.DataDirectory}",
                            StringComparison.InvariantCultureIgnoreCase))
                    .LogToAutodetectedLog()
                    .Build()
            };
        }
    }
}
