// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSDF.DataChecker.Domain.Resources
{
    public class CollectionCategoryResource
    {
        public CollectionCategoryResource() { }

        public CollectionCategoryResource(ContainerResource container)
        {
            Id = container.Id;
            Name = container.Name;
            IsDefault = container.IsDefault;
            ContainerTypeId = container.ContainerTypeId;

            ///CreatedByUserId = container.CreatedByUserId;
            EnvironmentType = container.EnvironmentType;
            RuleDetailsDestinationId = container.RuleDetailsDestinationId;

            //if (container.CommunityUser != null)
            //{
            //    OrganizationId = container.CommunityUser.CommunityOrganizationId ?? (new Guid());
            //    OrganizationName = container.CommunityUser.Organization.NameOfInstitution;
            //}

            Description = container.Description;
            OrganizationDescription = "";

            //if (container.CommunityUser != null)
            //{
            //    Email = container.CommunityUser.Email;
            //    UserName = container.CommunityUser.Name;
            //}

            ChildContainers = container.ChildContainers.Any()
                ? container.ChildContainers.Select(
                    m => new CollectionCategoryResource()
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        OrganizationDescription = "",
                        ContainerTypeId = m.ContainerTypeId,

                        //CreatedByUserId = m.CreatedByUserId
                    }).ToList()
                : new List<CollectionCategoryResource>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<CollectionCategoryResource> ChildContainers { get; set; } = new List<CollectionCategoryResource>();

        public List<RuleResource> Rules { get; set; } = new List<RuleResource>();

        public int ValidRules { get; set; }

        public int LastStatus { get; set; }

        public int AmountRules { get; set; }

        public bool IsDefault { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public int ContainerTypeId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationDescription { get; set; }

        public string OrganizationName { get; set; }

        public string Description { get; set; }

        public int EnvironmentType { get; set; }

        public List<TagResource> Tags { get; set; } = new List<TagResource>();

        public int? RuleDetailsDestinationId { get; set; }

        public ContainerDestinationResource ContainerDestination { get; set; }

        public Guid? ParentContainerId { get; set; }
    }
}
