// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MSDF.DataChecker.Domain.Resources
{
    public class RuleResource
    {
        public Guid Id { get; set; }

        public Guid ContainerId { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public string CreatedByUserName { get; internal set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ErrorMessage { get; set; }

        public int ErrorSeverityLevel { get; set; }

        public string Resolution { get; set; }

        public string DiagnosticSql { get; set; }

        public string Version { get; set; }

        public bool Enabled { get; set; }

        public string RuleIdentification { get; set; }

        public List<RuleExecutionLogResource> RuleExecutionLogs { get; set; }

        public List<TagResource> Tags { get; set; }

        public int Counter { get; set; }

        public int LastStatus { get; set; }

        public DateTime? LastExecution { get; set; }

        public bool TagIsInherited { get; set; }

        public int? CollectionRuleDetailsDestinationId { get; set; }

        public int? MaxNumberResults { get; set; }

        public string EnvironmentTypeText { get; set; }

        public Guid ParentContainer { get; set; }

        public string CollectionContainerName { get; set; }

        public string CollectionName { get; set; }

        public string ContainerName { get; set; }
    }
}
