// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain;
using MSDF.DataChecker.Domain.Entities.Enumeration;
using NUnit.Framework;
using Shouldly;

namespace MSDF.DataChecker.Tests.MappingTests.Enumeration
{
    [TestFixture]
    public class DatabaseEngineTypeMappingTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _db = SqliteMappingTestsSetup.DatabaseContext;
            _cancellationToken = SqliteMappingTestsSetup.CancellationToken;
        }

        private DatabaseContext _db;
        private CancellationToken _cancellationToken;

        [Test]
        public async Task Should_Add_databaseEngineType_With_Name_Only()
        {
            const string name = "Name Only";
            var databaseEngineType = new DatabaseEngineType {Name = name};

            await _db.DatabaseEngineTypes.AddAsync(databaseEngineType, _cancellationToken);
            await _db.SaveChangesAsync(_cancellationToken);

            var result = await _db.DatabaseEngineTypes.SingleOrDefaultAsync(x => x.Name == name, _cancellationToken);

            result.ShouldNotBeNull();
            result.DatabaseEngineTypeId.ShouldBeGreaterThan(0);
            result.Name.ShouldBe(name);
            result.Description.ShouldBeNull();
        }

        [Test]
        public async Task Should_Add_databaseEngineType_With_Name_And_Description()
        {
            const string name = "Name and Description";
            const string description = "Description with Name";

            var databaseEngineType = new DatabaseEngineType
            {
                Name = name,
                Description = description
            };

            await _db.DatabaseEngineTypes.AddAsync(databaseEngineType, _cancellationToken);
            await _db.SaveChangesAsync(_cancellationToken);

            var result = await _db.DatabaseEngineTypes.SingleOrDefaultAsync(x => x.Name == name, _cancellationToken);

            result.ShouldNotBeNull();
            result.DatabaseEngineTypeId.ShouldBeGreaterThan(0);
            result.Name.ShouldBe(name);
            result.Description.ShouldBe(description);
        }

        [Test]
        public async Task Should_Update_databaseEngineType()
        {
            const string name = "Updated Name";
            var databaseEngineType = new DatabaseEngineType {Name = name};

            await _db.DatabaseEngineTypes.AddAsync(databaseEngineType, _cancellationToken);
            await _db.SaveChangesAsync(_cancellationToken);

            var result = await _db.DatabaseEngineTypes.SingleOrDefaultAsync(x => x.Name == name, _cancellationToken);

            result.ShouldNotBeNull();
            result.DatabaseEngineTypeId.ShouldBeGreaterThan(0);
            result.Name.ShouldBe(name);
            result.Description.ShouldBeNull();

            result.Description = "Updated Description";

            _db.DatabaseEngineTypes.Update(result);
            await _db.SaveChangesAsync(_cancellationToken);

            var newResult = await _db.DatabaseEngineTypes.SingleOrDefaultAsync(x => x.Name == name, _cancellationToken);

            newResult.DatabaseEngineTypeId.ShouldBe(result.DatabaseEngineTypeId);
            newResult.Name.ShouldBe(result.Name);
            newResult.Description.ShouldBe("Updated Description");
        }

        [Test]
        public async Task Should_Delete_databaseEngineType()
        {
            const string name = "Deleted Name";
            var databaseEngineType = new DatabaseEngineType {Name = name};

            await _db.DatabaseEngineTypes.AddAsync(databaseEngineType, _cancellationToken);
            await _db.SaveChangesAsync(_cancellationToken);

            var result = await _db.DatabaseEngineTypes.SingleOrDefaultAsync(x => x.Name == name, _cancellationToken);

            result.ShouldNotBeNull();

            _db.DatabaseEngineTypes.Remove(result);
            await _db.SaveChangesAsync(_cancellationToken);

            var newResult = await _db.DatabaseEngineTypes.SingleOrDefaultAsync(x => x.Name == name, _cancellationToken);

            newResult.ShouldBeNull();
        }
    }
}
