// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using Hangfire.Storage;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Jobs
{
    public class JobsMappingProfile : Profile
    {
        public JobsMappingProfile()
        {
            CreateMap<RecurringJobDto, JobResource>()
                .ForMember(d => d.Status, opts => opts.Ignore())
                .ForMember(d => d.LastFinishedDateTime, opts => opts.Ignore())
                .ForMember(d => d.Name, opts => opts.Ignore())
                .ForMember(d => d.TypeName, opts => opts.Ignore())
                .ForMember(d => d.Type, opts => opts.Ignore())
                .ForMember(d => d.DatabaseEnvironmentId, opts => opts.Ignore())
                .ForMember(d => d.TagId, opts => opts.Ignore())
                .ForMember(d => d.ContainerId, opts => opts.Ignore());
        }
    }
}
