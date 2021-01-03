// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Hangfire;
using MediatR;

namespace MSDF.DataChecker.Domain.Services.Jobs.Commands
{
    public class Delete
    {
        public class Command : IRequest<Result<bool>>
        {
            public long Id { get; }

            public Command(long id) => Id = id;
        }

        public class Handler : RequestHandler<Command, Result<bool>>
        {
            protected override Result<bool> Handle(Command request)
            {
                if (request.Id < 1)
                    return Result<bool>.Fail("Invalid job id");

                RecurringJob.RemoveIfExists(request.Id.ToString());

                return Result<bool>.Success(true);
            }
        }
    }
}
