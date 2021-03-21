// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSDF.DataChecker.Domain.Entities.Enumeration;

namespace MSDF.DataChecker.Domain.Entities.Metadata
{
    [Table("Container", Schema = "dv_metadata")]
    public class Container
    {
        [Key]
        [Column("ContainerId")]
        public Guid ContainerId { get; set; } = Guid.NewGuid();

        [Column("Name")]
        public string Name { get; set; }

        // * ContainerTypeId : int <<FK>>
        [Column("ContainerTypeId")]
        public int ContainerTypeId { get; set; }

        [Column("ParentContainerId")]
        public Guid? ParentContainerId { get; set; }

        [Column("IsDefault")]
        public bool IsDefault { get; set; }

        [Column("Description")]
        [MaxLength(512)]
        public string Description { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("Modified")]
        public DateTime Modified { get; set; }

        // Navigation Properties
        public ContainerType ContainerType { get; set; }

        public Container ParentContainer { get; set; }

        public List<Container> Containers { get; set; } = new List<Container>();
    }
}
