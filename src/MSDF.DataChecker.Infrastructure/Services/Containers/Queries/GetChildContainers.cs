// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Containers.Queries
{
    public class GetChildContainers
    {
        public class Query : IRequest<Result<List<ContainerResource>>> { }

        public class Handler : IRequestHandler<Query, Result<List<ContainerResource>>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<ContainerResource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<ContainerResource> result = new List<ContainerResource>();

                var listParentContainers = await _db.Containers
                    .Where(m => m.ParentContainerId == null)
                    .Include(m => m.ChildContainers)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (listParentContainers != null && listParentContainers.Any())
                {
                    foreach (LegacyContainer parentContainer in listParentContainers)
                    {
                        if (parentContainer.ChildContainers != null && parentContainer.ChildContainers.Any())
                        {
                            foreach (LegacyContainer childContainer in parentContainer.ChildContainers)
                            {
                                result.Add(
                                    new ContainerResource()
                                    {
                                        Id = childContainer.Id,
                                        ParentContainerId = parentContainer.Id,
                                        Name = childContainer.Name,
                                        ParentContainerName = parentContainer.Name
                                    });
                            }
                        }
                    }
                }

                return Result<List<ContainerResource>>.Success(result);
            }
        }
    }
}
