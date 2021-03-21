// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain;
using MSDF.DataChecker.Domain.Entities.Metadata;
using NUnit.Framework;
using Shouldly;

namespace MSDF.DataChecker.Tests.MappingTests.Metadata
{
    [TestFixture]
    public class ContainerMappingTests
    {
        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _db = SqliteMappingTestsSetup.DatabaseContext;
            _cancellationToken = SqliteMappingTestsSetup.CancellationToken;
        }

        private DatabaseContext _db;
        private CancellationToken _cancellationToken;

        [Test]
        public async Task Should_create_a_container_with_success()
        {
            var container = FakedData.GenerateContainers().Single();

            await _db.Containers.AddAsync(container, _cancellationToken);
            await _db.SaveChangesAsync(_cancellationToken);

            var result = await _db.Containers.SingleOrDefaultAsync(x => x.ContainerId == container.ContainerId, _cancellationToken);

            result.ShouldNotBeNull();
            result.ContainerId.ShouldBe(container.ContainerId);
            result.Name.ShouldBe(container.Name);
            result.Description.ShouldBe(container.Description);
            result.ContainerTypeId.ShouldBe(container.ContainerTypeId);
            result.ContainerType.ShouldNotBeNull();
        }

        [Test]
        public async Task Should_create_a_container_with_child_containers_with_success()
        {
            var container = FakedData.GenerateContainersWithChildren(1, 5).Single();
            var childContainers = container.Containers;
            container.Containers = new List<Container>();

            await _db.Containers.AddAsync(container, _cancellationToken);
            await _db.Containers.AddRangeAsync(childContainers, _cancellationToken);
            await _db.SaveChangesAsync(_cancellationToken);

            var result = await _db.Containers
                .Include(x=> x.Containers)
                .SingleOrDefaultAsync(x => x.ContainerId == container.ContainerId, _cancellationToken);

            result.ShouldNotBeNull();
            result.ContainerId.ShouldBe(container.ContainerId);
            result.Name.ShouldBe(container.Name);
            result.Description.ShouldBe(container.Description);
            result.ContainerTypeId.ShouldBe(container.ContainerTypeId);
            result.ContainerType.ShouldNotBeNull();
            result.Containers.ShouldNotBeEmpty();
        }
    }
}
