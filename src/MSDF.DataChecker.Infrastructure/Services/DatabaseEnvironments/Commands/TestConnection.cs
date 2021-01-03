// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Commands
{
    public class TestConnection
    {
        public class Command : IRequest<Result<bool>>
        {
            public Command(DatabaseEnvironmentResource resource) => Resource = resource;

            public DatabaseEnvironmentResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger)
            {
                _logger = logger;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    await using var conn = new SqlConnection(request.Resource.GetConnectionString());

                    await conn.OpenAsync(cancellationToken);
                    return Result<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex.ToString());
                    return Result<bool>.Fail("Unable to connect to database.");
                }
            }
        }
    }
}
