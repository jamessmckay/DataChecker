// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Logs.Queries
{
    public class GetById
    {
        public class Query : IRequest<Result<LogResource>>
        {
            public Query(int id) => Id = id;

            public int Id { get; }
        }

        public class Handler : IRequestHandler<Query, Result<LogResource>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<LogResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var resource = await _db.Logs
                    .Where(x => x.Id == request.Id)
                    .ProjectTo<LogResource>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(cancellationToken);

                return Result<LogResource>.Success(resource);
            }
        }
    }
}
