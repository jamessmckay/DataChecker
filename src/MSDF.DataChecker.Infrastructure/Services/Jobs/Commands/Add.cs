// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using AutoMapper;
using Hangfire;
using Hangfire.Storage;
using MediatR;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Jobs.Commands
{
    public class Add
    {
        public class Command : IRequest<Result<long>>
        {
            public Command(JobResource resource) => Resource = resource;

            public JobResource Resource { get; }
        }

        public class Handler : RequestHandler<Command, Result<long>>
        {
            private readonly IMapper _mapper;

            public Handler(IMapper mapper)
            {
                _mapper = mapper;
            }

            protected override Result<long> Handle(Command request)
            {
                if (request.Resource == null)
                {
                    return Result<long>.Fail("Job resource is null");
                }

                if (!request.Resource.DatabaseEnvironmentId.HasValue)
                {
                    return Result<long>.Fail("Database Environment is required");
                }

                if (string.IsNullOrWhiteSpace(request.Resource.Cron))
                {
                    return Result<long>.Fail("Cron information is required");
                }

                request.Resource.Id = NewId();

                // TODO: This needs more evaluation
                RecurringJob.AddOrUpdate<JobRunner>(request.Resource.Id.ToString(), (j) => j.Run(request.Resource), request.Resource.Cron, System.TimeZoneInfo.Local);
                return Result<long>.Success(request.Resource.Id);

                long NewId()
                {
                    long maxJobId = JobStorage.Current
                        .GetConnection()
                        .GetRecurringJobs()
                        .Select(x => long.Parse(x.Id))
                        .Max(x => x);

                    return maxJobId++;
                }
            }
        }
    }
}
