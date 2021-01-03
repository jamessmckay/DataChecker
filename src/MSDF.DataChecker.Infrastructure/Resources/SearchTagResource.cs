// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MSDF.DataChecker.Domain.Resources
{
    public class SearchTagResource
    {
        public SearchTagResource()
        {
            TagsSelected = new List<TagResource>();
            Collections = new List<ContainerResource>();
            Containers = new List<ContainerResource>();
            Rules = new List<RuleResource>();
        }

        public List<TagResource> TagsSelected { get; set; }

        public List<ContainerResource> Collections { get; set; }

        public List<ContainerResource> Containers { get; set; }

        public List<RuleResource> Rules { get; set; }
    }
}
