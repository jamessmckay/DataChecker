// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Domain.Entities.Enumeration
{
    [Table("DatabaseEngineType", Schema = "dv_enumeration")]
    public class DatabaseEngineType
    {
        [Key]
        [Column("DatabaseEngineTypeId")]
        public int DatabaseEngineTypeId { get; set; }

        [Column("Name")]
        [MaxLength(256)]
        public string Name { get; set; }

        [Column("Description")]
        [MaxLength(512)]
        public string Description { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("Modified")]
        public DateTime Modified { get; set; }
    }
}