// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Tags.Commands
{
    public class SearchByTags
    {
        public class Command : IRequest<Result<SearchTagResource>>
        {
            public Command(List<TagResource> resources) => Resources = resources;

            public List<TagResource> Resources { get; }
        }

        public class Handler : IRequestHandler<Command, Result<SearchTagResource>>
        {
            public Task<Result<SearchTagResource>> Handle(Command request, CancellationToken cancellationToken)
            {
                //TODO: Add once the children services are completed.
                throw new NotImplementedException();
            }
        }
    }
}
