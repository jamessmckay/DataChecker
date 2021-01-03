// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MSDF.DataChecker.Domain.Services.UserParams.Commands
{
    public class Delete
    {
        public class Command : IRequest<Result<bool>>
        {
            public Command(Guid id) => Id = id;

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly DatabaseContext _db;

            public Handler(DatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.UserParams
                    .SingleOrDefaultAsync(rec => rec.Id == request.Id, cancellationToken);

                if (entity == null)
                {
                    return Result<bool>.Fail($"UserParam not found for '{request.Id}'");
                }

                _db.UserParams.Remove(entity);
                await _db.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
        }
    }
}
