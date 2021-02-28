// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Tests.MappingTests
{
    [Table("test", Schema = "test_schema")]
    public class TestTable
    {
        [Key]
        [Column("test_id")]
        public int Id { get; set; }

        [Column("test_name")]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
