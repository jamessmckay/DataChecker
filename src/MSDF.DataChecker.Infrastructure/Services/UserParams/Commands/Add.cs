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

namespace MSDF.DataChecker.Domain.Services.UserParams.Commands
{
    public class Add
    {
        public class Command : IRequest<Result<Guid>>
        {
            public Command(UserParamResource resource) => Resource = resource;

            public UserParamResource Resource { get; }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _db.UserParams
                    .SingleOrDefaultAsync(x => x.Id == request.Resource.Id, cancellationToken);

                bool isUpdated = entity != null;

                entity = _mapper.Map(request.Resource, entity);
                await _db.UserParams.AddAsync(entity, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                var result = Result<Guid>.Success(entity.Id);
                result.IsUpdated = isUpdated;

                return result;
            }
        }
    }
}
