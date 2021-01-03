// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using AutoMapper;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RuleExecutionLogs
{
    public class RuleExecutionLogMappingProfile : Profile
    {
        public RuleExecutionLogMappingProfile()
        {
            CreateMap<RuleExecutionLog, RuleTestResultResource>()
                .ForMember(d => d.Rule, opts => opts.Ignore())
                .ForMember(d => d.Status, opts => opts.MapFrom(s => Enum.GetName(typeof(Status), s.StatusId)))
                .ForMember(d => d.LastExecuted, opts => opts.Ignore())
                .ForMember(d => d.TestResults, opts => opts.Ignore())
                .ForMember(d => d.ErrorMessage, opts => opts.Ignore());
        }
    }
}
