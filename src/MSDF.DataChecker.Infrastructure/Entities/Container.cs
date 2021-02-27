// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Domain.Entities
{
    [Table("Containers", Schema = "datachecker")]
    public class Container : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ContainerTypeId { get; set; }

        public ContainerType ContainerType { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public Guid? ParentContainerId { get; set; }

        [ForeignKey("ParentContainerId")]
        public Container ParentContainer { get; set; }

        public virtual List<Container> ChildContainers { get; set; }

        public virtual List<Rule> Rules { get; set; }

        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public int EnvironmentType { get; set; }

        public int? RuleDetailsDestinationId { get; set; }

        [ForeignKey("RuleDetailsDestinationId")]
        public LegacyCatalog RuleDetailsDestination { get; set; }
    }

    [Table("ContainerTypes", Schema = "core")]
    public class ContainerType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
