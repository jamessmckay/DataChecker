// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Logs.Commands
{
    public class Add
    {
        public class Command : IRequest<Result<int>>
        {
            public Command(LogResource resource) => Resource = resource;

            public LogResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<LogResource, Log>(request.Resource);
                entity.DateCreated = DateTime.UtcNow;

                await _db.Logs.AddAsync(entity, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(entity.Id);
            }
        }
    }
}
