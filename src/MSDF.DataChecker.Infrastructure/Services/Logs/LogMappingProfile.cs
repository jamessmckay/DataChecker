// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Logs
{
    public class LogMappingProfile : Profile
    {
        public LogMappingProfile()
        {
            CreateMap<Log, LogResource>()
                .ReverseMap();
        }
    }
}
