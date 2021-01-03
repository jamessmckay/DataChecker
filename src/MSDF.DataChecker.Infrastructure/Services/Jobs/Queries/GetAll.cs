// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Hangfire.Storage;
using MediatR;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Jobs.Queries
{
    public class GetAll
    {
        public class Query : IRequest<Result<List<JobResource>>>
        {

        }

        public class Handler : RequestHandler<Query, Result<List<JobResource>>>
        {
            private readonly IMapper _mapper;

            public Handler(IMapper mapper)
            {
                _mapper = mapper;
            }

            protected override Result<List<JobResource>> Handle(Query request)
            {
                var result = JobStorage.Current
                    .GetConnection()
                    .GetRecurringJobs()
                    .Select(x => _mapper.Map<JobResource>(x))
                    .ToList();

                return Result<List<JobResource>>.Success(result);
            }
        }
    }
}
