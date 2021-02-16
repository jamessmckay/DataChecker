// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Domain;
using NUnit.Framework;

namespace MSDF.DataChecker.Tests.IntegrationTests
{
    [SetUpFixture]
    public class GlobalIntegrationTestsSetup
    {
        public static DatabaseContext DatabaseContext;
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

            DatabaseContext = new DatabaseContext(contextOptions);

            await DatabaseContext.Database.EnsureCreatedAsync();
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
