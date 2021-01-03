// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.UserParams.Queries
{
    public class GetByDatabaseEnvironmentId
    {
        public class Query : IRequest<Result<List<UserParamResource>>>
        {
            public Query(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserParamResource>>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<UserParamResource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var results = await _db.UserParams
                    .Where(rec => rec.DatabaseEnvironmentId == request.Id)
                    .ProjectTo<UserParamResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<UserParamResource>>.Success(results);
            }
        }
    }
}
