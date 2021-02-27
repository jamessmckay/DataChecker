// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Tags.Queries
{
    public class GetById
    {
        public class Query : IRequest<Result<TagResource>>
        {
            public Query(int id) => Id = id;

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<TagResource>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<TagResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var resource = await _db.Tags
                    .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                return Result<TagResource>.Success(resource);
            }
        }
    }
}
