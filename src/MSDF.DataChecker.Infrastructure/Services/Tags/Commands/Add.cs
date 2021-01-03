// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Tags.Commands
{
    public class Add
    {
        public class Command : IRequest<Result<int>>
        {
            public Command(TagResource resource) => Resource = resource;

            public TagResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.Tags
                    .SingleOrDefaultAsync(x => x.Id == request.Resource.Id, cancellationToken);

                bool isUpdated = entity != null;

                entity = _mapper.Map(request.Resource, entity);

                var created = DateTime.UtcNow;
                entity.Updated = created;

                if (!isUpdated)
                {
                    entity.Created = created;
                    await _db.Tags.AddAsync(entity, cancellationToken);
                }
                else
                {
                    _db.Tags.Update(entity);
                }

                await _db.SaveChangesAsync(cancellationToken);

                var result = Result<int>.Success(entity.Id);
                result.IsUpdated = isUpdated;

                return result;
            }
        }
    }
}
