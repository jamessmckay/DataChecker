// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Containers.Queries
{
    public class GetToCommunity
    {
        public class Query : IRequest<Result<CollectionCategoryResource>>
        {
            public Query(Guid id) => Id = id;

            public Guid Id { get; }
        }

        // TODO Address n+1 issues
        public class Handler : IRequestHandler<Query, Result<CollectionCategoryResource>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<CollectionCategoryResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var container = await _db.Containers
                    .Include(m => m.ChildContainers)
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (container == null)
                {
                    return Result<CollectionCategoryResource>.Success(null);
                }

                var collectionCategory = new CollectionCategoryResource
                {
                    Id = container.Id,
                    Name = container.Name,
                    Description = container.Description,
                    EnvironmentType = container.EnvironmentType,
                    ContainerTypeId = container.ContainerTypeId,
                    Tags = await _db.TagEntities
                        .Where(x => x.ContainerId == container.Id)
                        .Select(x => x.Tag)
                        .Where(x => x.IsPublic)
                        .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken),
                    RuleDetailsDestinationId = container.RuleDetailsDestinationId
                };

                var environmentCatalog = await _db.Catalogs
                    .ProjectTo<CatalogResource>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(x => x.Id == container.EnvironmentType, cancellationToken);

                if (collectionCategory.RuleDetailsDestinationId != null)
                {
                    var existCatalog = await _db.Catalogs
                        .ProjectTo<CatalogResource>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(x => x.Id == collectionCategory.RuleDetailsDestinationId.Value, cancellationToken);

                    if (existCatalog != null)
                    {
                        var listColumns =
                            await GetColumnsByTableAsync(existCatalog.Name, "destination");

                        if (listColumns != null)
                        {
                            collectionCategory.ContainerDestination = new ContainerDestinationResource
                            {
                                CatalogId = existCatalog.Id,
                                ContainerId = collectionCategory.Id,
                                DestinationName = existCatalog.Name,
                                DestinationStructure = JsonSerializer.Serialize(listColumns)
                            };
                        }
                    }
                }

                foreach (var itemContainer in container.ChildContainers)
                {
                    var category = new CollectionCategoryResource()
                    {
                        Id = itemContainer.Id,
                        Name = itemContainer.Name,
                        Description = itemContainer.Description,
                        ParentContainerId = itemContainer.ParentContainerId,
                        Rules = await _db.Rules
                            .Where(x => x.ContainerId == itemContainer.Id)
                            .ProjectTo<RuleResource>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken),
                        Tags = await _db.TagEntities
                            .Where(x => x.ContainerId == itemContainer.Id)
                            .Select(x => x.Tag)
                            .Where(x => x.IsPublic)
                            .ProjectTo<TagResource>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken)
                    };

                    collectionCategory.ChildContainers.Add(category);
                }

                return Result<CollectionCategoryResource>.Success(collectionCategory);

                async Task<List<DestinationTableColumn>> GetColumnsByTableAsync(string tableName, string tableSchema)
                {
                    var columns = new List<DestinationTableColumn>();

                    string connectionString = _db.Database.GetDbConnection().ConnectionString;

                    await using SqlConnection destinationConnection = new SqlConnection(connectionString);

                    await destinationConnection.OpenAsync(cancellationToken);

                    string sql = string.Format(
                        "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE " +
                        "FROM INFORMATION_SCHEMA.COLUMNS " +
                        "WHERE TABLE_NAME = @tablename AND TABLE_SCHEMA = @tableschema " +
                        "ORDER BY ORDINAL_POSITION");

                    await using var sqlCommand = new SqlCommand(sql, destinationConnection);

                    sqlCommand.Parameters.AddWithValue("@tablename", tableName);
                    sqlCommand.Parameters.AddWithValue("@tableschema", tableSchema);
                    var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        columns.Add(
                            new DestinationTableColumn
                            {
                                Name = reader.GetValue(0).ToString().ToLower(),
                                Type = reader.GetValue(1).ToString().ToLower(),
                                IsNullable = reader.GetValue(2).ToString().ToLower() != "no"
                            });
                    }

                    return columns;
                }
            }
        }
    }
}
