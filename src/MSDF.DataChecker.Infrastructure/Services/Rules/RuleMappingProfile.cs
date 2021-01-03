// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Rules
{
    public class RuleMappingProfile : Profile
    {
        public RuleMappingProfile()
        {
            CreateMap<Rule, RuleResource>()
                .ForMember(x => x.CreatedByUserId, opts => opts.Ignore())
                .ForMember(x => x.CreatedByUserName, opts => opts.Ignore())
                .ForMember(x => x.Tags, opts => opts.Ignore())
                .ForMember(x => x.Counter, opts => opts.Ignore())
                .ForMember(x => x.LastStatus, opts => opts.Ignore())
                .ForMember(x => x.LastExecution, opts => opts.Ignore())
                .ForMember(x => x.TagIsInherited, opts => opts.Ignore())
                .ForMember(x => x.CollectionRuleDetailsDestinationId, opts => opts.Ignore())
                .ForMember(x => x.EnvironmentTypeText, opts => opts.Ignore())
                .ForMember(x => x.ParentContainer, opts => opts.Ignore())
                .ForMember(x => x.CollectionContainerName, opts => opts.Ignore())
                .ForMember(x => x.CollectionName, opts => opts.Ignore())
                .ForMember(x => x.ContainerName, opts => opts.Ignore())
                .ForMember(x => x.Enabled, opts => opts.Ignore())
                .ForMember(x => x.RuleExecutionLogs, opts => opts.Ignore())
                .ReverseMap();
        }
    }
}
