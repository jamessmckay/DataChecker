// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using Bogus;
using MSDF.DataChecker.Domain.Entities.Metadata;

namespace MSDF.DataChecker.Tests
{
    public static class FakedData
    {
        public static List<Container> GenerateContainers(int count = 1)
        {
            return new Faker<Container>()
                .RuleFor(x => x.Name, f => f.System.FileName())
                .RuleFor(x => x.Description, f => f.Hacker.Phrase())
                .RuleFor(x => x.ContainerTypeId, f => f.Random.Int(1, 2))
                .Generate(count);
        }

        public static List<Container> GenerateContainersWithChildren(int containerCount = 1, int childContainerCount = 1)
        {
            var parentContainers = new Faker<Container>()
                .RuleFor(x => x.Name, f => f.System.FileName())
                .RuleFor(x => x.Description, f => f.Hacker.Phrase())
                .RuleFor(x => x.ContainerTypeId, f => 1)
                .Generate(containerCount);

            foreach (Container container in parentContainers)
            {
                container.Containers = new Faker<Container>()
                    .RuleFor(x => x.Name, f => f.System.FileName())
                    .RuleFor(x => x.Description, f => f.Hacker.Phrase())
                    .RuleFor(x => x.ContainerTypeId, f => 2)
                    .RuleFor(x => x.ParentContainerId, f => container.ContainerId)
                    .Generate(childContainerCount);
            }

            return parentContainers;
        }
    }
}
