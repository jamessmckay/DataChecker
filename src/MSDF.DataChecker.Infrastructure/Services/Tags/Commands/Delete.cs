// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MSDF.DataChecker.Domain.Services.Tags.Commands
{
    public class Delete
    {
        public class Command : IRequest<Result<bool>>
        {
            public Command(int id) => Id = id;

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly LegacyDatabaseContext _db;

            public Handler(LegacyDatabaseContext db)
            {
                _db = db;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.Tags
                    .SingleOrDefaultAsync(rec => rec.Id == request.Id, cancellationToken);

                var tagEntities = await _db.TagEntities
                    .Where(rec => rec.TagId == request.Id)
                    .ToListAsync(cancellationToken);

                if (tagEntities.Any())
                {
                    _db.TagEntities.RemoveRange(tagEntities);
                }

                if (entity != null)
                {
                    _db.Tags.Remove(entity);
                }

                await _db.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
        }
    }
}
