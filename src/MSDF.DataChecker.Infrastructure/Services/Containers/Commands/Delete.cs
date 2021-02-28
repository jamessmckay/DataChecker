// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;

namespace MSDF.DataChecker.Domain.Services.Containers.Commands
{
    public class Delete
    {
        public class Command : IRequest<Result<bool>>
        {
            public Command(Guid id) => Id = id;

            public Guid Id { get; }
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
                var container = await _db.Containers
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (container == null)
                {
                    return Result<bool>.Fail("Container does not exist");
                }

                var childContainers = await _db.Containers
                    .Where(x => x.ParentContainerId != null && x.ParentContainerId == request.Id)
                    .ToListAsync(cancellationToken);

                var childContainersId = new List<Guid>();
                var allRules = new List<Rule>();
                var ruleLogs = new List<RuleExecutionLog>();
                var allTags = new List<TagLegacyEntity>();

                var rules = await _db.Rules.Where(x => childContainersId.Contains(x.ContainerId)).ToListAsync(cancellationToken);
                var allRulesId = rules.Select(x => x.Id).ToList();

                allRules.AddRange(rules);

                if (childContainers.Any())
                {
                    // TODO: this should really be a cascade with the ORM and Database
                    childContainersId.AddRange(childContainers.Select(x => x.Id));

                    ruleLogs.AddRange(
                        await _db.RuleExecutionLogs.Where(x => allRulesId.Contains(x.RuleId))
                            .ToListAsync(cancellationToken));

                    allTags.AddRange(
                        await _db.TagEntities
                            .Where(x => x.ContainerId.HasValue && childContainersId.Contains(x.ContainerId.Value))
                            .ToListAsync(cancellationToken));

                    allTags.AddRange(
                        await _db.TagEntities
                            .Where(x => x.RuleId.HasValue && allRulesId.Contains(x.RuleId.Value))
                            .ToListAsync(cancellationToken));
                }
                else
                {
                    childContainersId.Add(request.Id);

                    ruleLogs.AddRange(
                        await _db.RuleExecutionLogs
                            .Where(x => allRulesId.Contains(x.RuleId))
                            .ToListAsync(cancellationToken));

                    allTags.AddRange(
                        await _db.TagEntities
                            .Where(x => x.ContainerId != null && childContainersId.Contains(x.ContainerId.Value))
                            .ToListAsync(cancellationToken));

                    allTags.AddRange(
                        await _db.TagEntities
                            .Where(x => x.RuleId != null && allRulesId.Contains(x.RuleId.Value))
                            .ToListAsync(cancellationToken));
                }

                allTags.AddRange(
                    _db.TagEntities.Where(x => x.ContainerId != null && x.ContainerId == request.Id).ToList());

                _db.RuleExecutionLogs.RemoveRange(ruleLogs);
                _db.TagEntities.RemoveRange(allTags);
                _db.Rules.RemoveRange(allRules);
                _db.Containers.RemoveRange(childContainers);
                await _db.SaveChangesAsync(cancellationToken);

                _db.Containers.Remove(container);

                await _db.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
        }
    }
}
