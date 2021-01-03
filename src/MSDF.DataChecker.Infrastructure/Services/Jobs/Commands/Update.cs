// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using Hangfire;
using MediatR;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Jobs.Commands
{
    public class Update
    {
        public class Command : IRequest<Result<bool>>
        {
            public JobResource Resource { get; }

            public Command(JobResource resource) => Resource = resource;
        }

        public class Handler : RequestHandler<Command, Result<bool>>
        {
            private readonly IMapper _mapper;

            public Handler(IMapper mapper)
            {
                _mapper = mapper;
            }

            protected override Result<bool> Handle(Command request)
            {
                if (request.Resource == null)
                {
                    return Result<bool>.Fail("Job resource is null");
                }

                if (!request.Resource.DatabaseEnvironmentId.HasValue)
                {
                    return Result<bool>.Fail("Database Environment is required");
                }

                if (string.IsNullOrWhiteSpace(request.Resource.Cron))
                {
                    return Result<bool>.Fail("Cron information is required");
                }

                if (request.Resource.Id < 1)
                {
                    return Result<bool>.Fail("Job Id is not valid");
                }

                // TODO: This needs more evaluation
                RecurringJob.AddOrUpdate<JobRunner>(request.Resource.Id.ToString(), (j) => j.Run(request.Resource), request.Resource.Cron, System.TimeZoneInfo.Local);
                return Result<bool>.Success(true);

            }
        }
    }
}
