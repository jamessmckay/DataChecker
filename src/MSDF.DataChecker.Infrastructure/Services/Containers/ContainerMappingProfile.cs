// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Containers
{
    public class ContainerMappingProfile : Profile
    {
        public ContainerMappingProfile()
        {
            CreateMap<LegacyContainer, ContainerResource>()
                .ForMember(d => d.CommunityUser, opts => opts.Ignore())
                .ForMember(d => d.Tags, opts => opts.Ignore())
                .ForMember(d => d.TagIsInherited, opts => opts.Ignore())
                .ForMember(d => d.ContainerDestination, opts => opts.Ignore())
                .ForMember(d => d.CreateNewCollection, opts => opts.Ignore())
                .ForMember(d => d.CatalogEnvironmentType, opts => opts.Ignore())
                .ForMember(d => d.ContainerType, opts => opts.Ignore());
        }
    }
}
