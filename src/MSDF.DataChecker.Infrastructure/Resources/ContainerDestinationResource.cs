// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MSDF.DataChecker.Domain.Resources
{
    public class ContainerDestinationResource
    {
        public int Id { get; set; }

        public Guid ContainerId { get; set; }

        public int CatalogId { get; set; }

        public string DestinationName { get; set; }

        public string DestinationStructure { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public Guid UserId { get; set; }

        public int Version { get; set; }
    }
}
