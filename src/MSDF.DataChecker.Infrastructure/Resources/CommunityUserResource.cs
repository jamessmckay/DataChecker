// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MSDF.DataChecker.Domain.Resources
{
    public class CommunityUserResource
    {
        public CommunityUserResource()
        {
            CreatedDate = DateTime.UtcNow;
            LastUpdatedDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public Guid CreateByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public long? LastUpdatedUserId { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public Guid? CommunityOrganizationId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public CommunityOrganizationResource Organization { get; set; }
    }
}
