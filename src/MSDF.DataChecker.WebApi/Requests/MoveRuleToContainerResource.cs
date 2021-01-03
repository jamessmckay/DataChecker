// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.WebApi.Requests
{
    public class MoveRuleToContainerResource
    {
        public List<Guid> Rules { get; set; }
        public ContainerResource ContainerTo { get; set; }
    }
}
