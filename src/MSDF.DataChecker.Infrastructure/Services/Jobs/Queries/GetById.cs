// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

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
    public class GetById
    {
        public class Query : IRequest<Result<JobResource>>
        {
            public string Id { get; }

            public Query(string id)
            {
                Id = id;
            }
        }

        public class Handler : RequestHandler<Query, Result<JobResource>>
        {
            private readonly IMapper _mapper;

            public Handler(IMapper mapper)
            {
                _mapper = mapper;
            }

            protected override Result<JobResource> Handle(Query request)
            {
                var result = JobStorage.Current
                    .GetConnection()
                    .GetRecurringJobs()
                    .FirstOrDefault(x => x.Id == request.Id);


                return Result<JobResource>.Success(_mapper.Map<JobResource>(result));
            }
        }
    }
}
