// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MSDF.DataChecker.WebApi.Requests
{
    public class RulesCopyToContainerByIdsResource
    {
        public List<Guid> RuleIds { get; set; }
        public List<Guid> ContainerIds { get; set; }
    }
}
