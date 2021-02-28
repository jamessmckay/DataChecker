﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Domain.Entities
{
    [Table("UserParams", Schema = "datachecker")]
    public class UserParam : ILegacyEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Guid DatabaseEnvironmentId { get; set; }
    }
}
