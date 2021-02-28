﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Domain.Entities
{
    [Table("Catalogs", Schema = "core")]
    public class LegacyCatalog : ILegacyEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CatalogType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Updated { get; set; }
    }
}
