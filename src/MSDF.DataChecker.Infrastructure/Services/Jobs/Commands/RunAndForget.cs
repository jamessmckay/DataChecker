// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Hangfire;
using MediatR;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Jobs.Commands
{
    public class RunAndForget
    {
        public class Command : IRequest<Result<bool>>
        {
            public JobResource Resource { get; }

            public Command(JobResource resource) => Resource = resource;
        }

        public class Handler : RequestHandler<Command,Result<bool>>
        {
            protected override Result<bool> Handle(Command request)
            {
                if (request.Resource == null)
                {
                    return Result<bool>.Fail("Job is null");
                }

                if (!request.Resource.DatabaseEnvironmentId.HasValue)
                {
                    return Result<bool>.Fail("Database Environment is required");
                }

                BackgroundJob.Enqueue<JobRunner>((jr) => jr.Run(request.Resource));

                return Result<bool>.Success(true);
            }
        }
    }
}
