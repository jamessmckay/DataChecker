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

namespace MSDF.DataChecker.Domain.Services.Containers.Queries
{
    public class GetByName
    {
        public class Query : IRequest<Result<ContainerResource>>
        {
            public Query(string name) => Name = name;

            public string Name { get; }
        }

        public class Handler : IRequestHandler<Query, Result<ContainerResource>>
        {
            private readonly DatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<ContainerResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await _db.Containers
                    .ProjectTo<ContainerResource>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(rec => rec.Name.ToLower() == request.Name.ToLower(), cancellationToken);

                if (result == null)
                {
                    return Result<ContainerResource>.Fail($"Container not found for '{request.Name}';");
                }

                return Result<ContainerResource>.Success(result);
            }
        }
    }
}
