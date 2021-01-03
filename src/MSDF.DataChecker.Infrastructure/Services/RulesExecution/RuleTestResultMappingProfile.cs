// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.RulesExecution
{
    public class RuleTestResultMappingProfile : Profile
    {
        public RuleTestResultMappingProfile()
        {
            CreateMap<RuleTestResult, RuleTestResultResource>()
                .ForMember(m => m.LastExecuted, opts => opts.Ignore())
                .ForMember(m => m.TestResults, opts => opts.Ignore())
                .ForMember(m => m.ErrorMessage, opts => opts.Ignore());
        }
    }
}
