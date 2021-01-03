// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MSDF.DataChecker.Domain.Resources
{
    public class JobResource
    {
        public long Id { get; set; }
        public string Cron { get; set; }
        public string Status { get; set; }
        public DateTime? LastFinishedDateTime { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public JobType Type { get; set; }
        public Guid? DatabaseEnvironmentId { get; set; }
        public int? TagId { get; set; }
        public Guid? ContainerId { get; set; }

        public enum JobType
        {
            Tag = 1,
            Container
        }
    }
}
