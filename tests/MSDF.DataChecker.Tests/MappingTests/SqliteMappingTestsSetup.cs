// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Common.Enumerations;
using MSDF.DataChecker.Domain;
using MSDF.DataChecker.Domain.Entities.Enumeration;
using NUnit.Framework;

namespace MSDF.DataChecker.Tests.MappingTests
{
    [SetUpFixture]
    public class SqliteMappingTestsSetup
    {
        public static DatabaseContext DatabaseContext;

        public static CancellationToken CancellationToken;

        private string _databaseFileName;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _databaseFileName = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Path.GetRandomFileName()}.db");

            var connectionStringBuilder = new SqliteConnectionStringBuilder($"Data Source={_databaseFileName};Cache=Shared");

            var contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(connectionStringBuilder.ConnectionString)
                .LogTo(x => Debug.WriteLine(x), LogLevel.Information)
                .Options;

            DatabaseContext = new SqliteDatabaseContext(contextOptions, DatabaseEngine.SQLite);

            CancellationToken = new CancellationToken();

            await DatabaseContext.Database.EnsureCreatedAsync(CancellationToken);

            await SeedTestData();
        }

        private async Task SeedTestData()
        {
            await DatabaseContext.ContainerTypes
                .AddRangeAsync(SeedData.ContainerTypes(), CancellationToken);

            await DatabaseContext.SaveChangesAsync(CancellationToken);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DatabaseContext?.Dispose();

            if (File.Exists(_databaseFileName))
            {
                File.Delete(_databaseFileName);
            }
        }
    }
}
