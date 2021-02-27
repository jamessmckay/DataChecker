// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Catalogs.Queries
{
    public class GetAll
    {
        public class Query : IRequest<Result<List<CatalogResource>>> { }

        public class Handler : IRequestHandler<Query, Result<List<CatalogResource>>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<CatalogResource>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var catalogs = await _db.Catalogs
                    .ProjectTo<CatalogResource>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<CatalogResource>>.Success(catalogs);
            }
        }
    }
}
