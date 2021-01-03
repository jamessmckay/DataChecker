// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MSDF.DataChecker.Domain.Resources
{
    public class ContainerResource
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ContainerTypeId { get; set; }

        public ContainerTypeResource ContainerType { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public Guid? ParentContainerId { get; set; }

        public List<ContainerResource> ChildContainers { get; set; } = new List<ContainerResource>();

        public List<RuleResource> Rules { get; set; } = new List<RuleResource>();

        public bool IsDefault { get; set; }

        public CommunityUserResource CommunityUser { get; set; }

        public string Description { get; set; }

        public int EnvironmentType { get; set; }

        public List<TagResource> Tags { get; set; } = new List<TagResource>();

        public bool TagIsInherited { get; set; }

        public string ParentContainerName { get; set; }

        public int? RuleDetailsDestinationId { get; set; }

        public ContainerDestinationResource ContainerDestination { get; set; }

        public CatalogResource CatalogEnvironmentType { get; set; }

        public bool CreateNewCollection { get; set; }
    }
}
