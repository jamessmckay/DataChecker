// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace MSDF.DataChecker.Tests.IntegrationTests
{
    [TestFixture]
    public class SqliteDeployTests
    {
        [Test]
        public async Task ShouldDeployTheDatabaseWithSchemaNamesAttachedToTheTables()
        {
            var db = (SqliteDatabaseContext) GlobalIntegrationTestsSetup.DatabaseContext;
            var cancellationToken = GlobalIntegrationTestsSetup.CancellationToken;

            await db.TestTables.AddAsync(new TestTable { Name = "Test Data"}, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            var result = await db.TestTables.SingleOrDefaultAsync(x => x.Name == "Test Data", cancellationToken);

            result.ShouldNotBeNull();
            result.Id.ShouldBeGreaterThan(0);
            result.Name.ShouldBe("Test Data");
        }
    }
}
