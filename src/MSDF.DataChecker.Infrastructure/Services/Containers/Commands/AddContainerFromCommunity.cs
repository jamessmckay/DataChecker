// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Services.Containers.Commands
{
    public class AddContainerFromCommunity
    {
        public class Command : IRequest<Result<string>>
        {
            public Command(ContainerResource resource) => Resource = resource;

            public ContainerResource Resource { get; }
        }

        // TODO Address n+1, Should we be deleting? Post by definition adds a new object always.
        // TODO Need to understand the process better
        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly LegacyDatabaseContext _db;
            private readonly IMapper _mapper;

            public Handler(LegacyDatabaseContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                // string result = string.Empty;
                //
                // try
                // {
                //     var model = request.Resource;

                // if (!model.CreateNewCollection)
                // {
                //     var collectionToDelete = await GetByNameAsync(model);
                //     if (collectionToDelete != null)
                //         await DeleteAsync(collectionToDelete.Id);
                // }

                //     string collectionName = model.Name;
                //     int counter = 1;
                //     while (true)
                //     {
                //
                //         var existCollection = await _db.Containers.SingleOrDefaultAsync(
                //             rec => rec.Name.ToLower() == request.Resource.Name.ToLower(), cancellationToken);
                //
                //         if (existCollection == null) break;
                //         model.Name = $"{collectionName} - ({counter})";
                //         counter++;
                //     }
                //
                //     var catalogsInformation = await _catalogQueries.GetAsync();
                //     var existEnvironment = catalogsInformation.FirstOrDefault(rec => rec.CatalogType == "EnvironmentType" && rec.Name.ToLower() == model.CatalogEnvironmentType.Name.ToLower());
                //     if (existEnvironment != null)
                //         model.EnvironmentType = existEnvironment.Id;
                //     else
                //     {
                //         var newEnvironmentType = await _catalogService.AddAsync(new CatalogBO
                //         {
                //             CatalogType = "EnvironmentType",
                //             Description=model.CatalogEnvironmentType.Name,
                //             Name=model.CatalogEnvironmentType.Name
                //         });
                //         model.EnvironmentType = newEnvironmentType.Id;
                //     }
                //
                //     if (model.RuleDetailsDestinationId != null && model.ContainerDestination != null && !string.IsNullOrEmpty(model.ContainerDestination.DestinationName))
                //     {
                //         bool createDestinationTable = true;
                //         var existDestinationTable = catalogsInformation.FirstOrDefault(rec => rec.CatalogType == "RuleDetailsDestinationType" && rec.Name.ToLower() == model.ContainerDestination.DestinationName.ToLower());
                //         if (existDestinationTable != null)
                //         {
                //             List<DestinationTableColumn> destinationTableInDbColumns = JsonSerializer.Deserialize<List<DestinationTableColumn>>(model.ContainerDestination.DestinationStructure);
                //             var listColumnsFromDestination = await _edFiRuleExecutionLogDetailQueries.GetColumnsByTableAsync(model.ContainerDestination.DestinationName, "destination");
                //
                //             if (destinationTableInDbColumns.Count == listColumnsFromDestination.Count)
                //             {
                //                 createDestinationTable = false;
                //                 foreach (var columnInDestinationTable in destinationTableInDbColumns)
                //                 {
                //                     var existColumn = listColumnsFromDestination.FirstOrDefault(rec =>
                //                     rec.Name.ToLower() == columnInDestinationTable.Name.ToLower() &&
                //                     rec.Type == columnInDestinationTable.Type.ToLower() &&
                //                     rec.IsNullable == columnInDestinationTable.IsNullable);
                //
                //                     if (existColumn == null)
                //                     {
                //                         createDestinationTable = true;
                //                         break;
                //                     }
                //                 }
                //                 if (!createDestinationTable)
                //                     model.RuleDetailsDestinationId = existDestinationTable.Id;
                //                 else
                //                 {
                //                     int counterTable = 1;
                //                     string newDestinationTableName = $"{model.ContainerDestination.DestinationName}_{counterTable}";
                //                     while (true)
                //                     {
                //                         existDestinationTable = catalogsInformation.FirstOrDefault(rec =>
                //                         rec.CatalogType == "RuleDetailsDestinationType" &&
                //                         rec.Name.ToLower() == newDestinationTableName.ToLower());
                //                         if (existDestinationTable == null) break;
                //                         counterTable++;
                //                         newDestinationTableName = $"{model.ContainerDestination.DestinationName}_{counterTable}";
                //                     }
                //                     model.ContainerDestination.DestinationName = newDestinationTableName;
                //                 }
                //             }
                //         }
                //
                //         if(createDestinationTable)
                //         {
                //             var newDestinationTable = await _catalogService.AddAsync(new CatalogBO
                //             {
                //                 CatalogType = "RuleDetailsDestinationType",
                //                 Description = model.ContainerDestination.DestinationName,
                //                 Name = model.ContainerDestination.DestinationName
                //             });
                //             model.RuleDetailsDestinationId = newDestinationTable.Id;
                //
                //             bool existDestinationTableInDb = await _edFiRuleExecutionLogDetailQueries.ExistExportTableFromRuleExecutionLogAsync(model.ContainerDestination.DestinationName, "destination");
                //             if (!existDestinationTableInDb)
                //             {
                //                 List<DestinationTableColumn> destinationTableInDbColumns = JsonSerializer.Deserialize<List<DestinationTableColumn>>(model.ContainerDestination.DestinationStructure);
                //                 List<string> sqlColumns = new List<string>();
                //                 foreach (var column in destinationTableInDbColumns)
                //                 {
                //                     string isNull = column.IsNullable ? "NULL" : "NOT NULL";
                //                     if (column.Name == "id" && column.Type == "int")
                //                         sqlColumns.Add("[Id] [int] IDENTITY(1,1) NOT NULL");
                //                     else if (column.Type.Contains("varchar"))
                //                         sqlColumns.Add($"[{column.Name}] [{column.Type}](max) {isNull}");
                //                     else if (column.Type.Contains("datetime"))
                //                         sqlColumns.Add($"[{column.Name}] [{column.Type}](7) {isNull}");
                //                     else
                //                         sqlColumns.Add($"[{column.Name}] [{column.Type}] {isNull}");
                //                 }
                //                 string sqlCreate = $"CREATE TABLE [destination].[{model.ContainerDestination.DestinationName}]({string.Join(",", sqlColumns)}) ";
                //                 await _edFiRuleExecutionLogDetailCommands.ExecuteSqlAsync(sqlCreate);
                //             }
                //         }
                //     }
                //
                //     Container newCollection = new Container
                //     {
                //         ContainerTypeId = 1,
                //         Description = model.Description,
                //         Name = model.Name,
                //         IsDefault = false,
                //         EnvironmentType = model.EnvironmentType,
                //         RuleDetailsDestinationId = model.RuleDetailsDestinationId,
                //         ChildContainers = new List<Container>()
                //     };
                //
                //     if (model.ChildContainers != null && model.ChildContainers.Any())
                //     {
                //         model.ChildContainers.ForEach(rec => {
                //             Container newChildContainer = new Container
                //             {
                //                 ContainerTypeId = 2,
                //                 Description = rec.Description,
                //                 Name = rec.Name,
                //                 IsDefault = false,
                //                 EnvironmentType = 0,
                //                 RuleDetailsDestinationId = null,
                //                 Rules= new List<Rule>()
                //             };
                //
                //             if (rec.Rules != null && rec.Rules.Any())
                //             {
                //                 rec.Rules.ForEach(recRule =>
                //                 {
                //                     newChildContainer.Rules.Add(new Rule
                //                     {
                //                         Description = recRule.Description,
                //                         DiagnosticSql = recRule.DiagnosticSql,
                //                         //Enabled = true,
                //                         ErrorMessage = recRule.ErrorMessage,
                //                         MaxNumberResults = recRule.MaxNumberResults,
                //                         Name = recRule.Name,
                //                         ErrorSeverityLevel = recRule.ErrorSeverityLevel,
                //                         Resolution = recRule.Resolution,
                //                         RuleIdentification = recRule.RuleIdentification,
                //                         Version = recRule.Version
                //                     });
                //                 });
                //             }
                //
                //             newCollection.ChildContainers.Add(newChildContainer);
                //         });
                //     }
                //
                //     var newCollectionFromDatabase = await _collectionCommands.AddAsync(newCollection);
                //     if (newCollectionFromDatabase != null)
                //     {
                //         var listTags = await _tagQueries.GetAsync();
                //
                //         if (model.Tags != null && model.Tags.Any())
                //         {
                //             foreach (var tag in model.Tags)
                //             {
                //                 var existTag = listTags.FirstOrDefault(rec => rec.Name.ToLower() == tag.Name.ToLower());
                //                 if (existTag == null)
                //                 {
                //                     existTag = await _tagCommands.AddAsync(new Tag {
                //                         Description=tag.Description,
                //                         IsPublic=true,
                //                         Name=tag.Name.ToUpper()
                //                     });
                //                     listTags.Add(existTag);
                //                 }
                //
                //                 await _tagCommands.AddTagToEntityAsync(new TagEntity {
                //                     ContainerId = newCollectionFromDatabase.Id,
                //                     TagId = existTag.Id
                //                 });
                //             }
                //         }
                //
                //         if (model.ChildContainers != null && model.ChildContainers.Any())
                //         {
                //             foreach (var childContainer in model.ChildContainers)
                //             {
                //                 var newChildContainer = newCollectionFromDatabase.ChildContainers.FirstOrDefault(rec => rec.Name == childContainer.Name);
                //                 if (childContainer.Tags != null && childContainer.Tags.Any())
                //                 {
                //                     foreach (var tag in childContainer.Tags)
                //                     {
                //                         var existTag = listTags.FirstOrDefault(rec => rec.Name.ToLower() == tag.Name.ToLower());
                //                         if (existTag == null)
                //                         {
                //                             existTag = await _tagCommands.AddAsync(new Tag
                //                             {
                //                                 Description = tag.Description,
                //                                 IsPublic = true,
                //                                 Name = tag.Name.ToUpper()
                //                             });
                //                             listTags.Add(existTag);
                //                         }
                //
                //                         await _tagCommands.AddTagToEntityAsync(new TagEntity
                //                         {
                //                             ContainerId = newChildContainer.Id,
                //                             TagId = existTag.Id
                //                         });
                //                     }
                //                 }
                //
                //                 if (childContainer.Rules != null && childContainer.Rules.Any())
                //                 {
                //                     foreach (var rule in childContainer.Rules)
                //                     {
                //                         var newRule = newChildContainer.Rules.FirstOrDefault(rec=>rec.Name==rule.Name);
                //                         if (rule.Tags != null && rule.Tags.Any())
                //                         {
                //                             foreach (var tag in rule.Tags)
                //                             {
                //                                 var existTag = listTags.FirstOrDefault(rec => rec.Name.ToLower() == tag.Name.ToLower());
                //                                 if (existTag == null)
                //                                 {
                //                                     existTag = await _tagCommands.AddAsync(new Tag
                //                                     {
                //                                         Description = tag.Description,
                //                                         IsPublic = true,
                //                                         Name = tag.Name.ToUpper()
                //                                     });
                //                                     listTags.Add(existTag);
                //                                 }
                //
                //                                 await _tagCommands.AddTagToEntityAsync(new TagEntity
                //                                 {
                //                                     RuleId = newRule.Id,
                //                                     TagId = existTag.Id
                //                                 });
                //                             }
                //                         }
                //                     }
                //                 }
                //             }
                //         }
                //     }
                // }
                // catch (Exception ex)
                // {
                //     result = ex.Message;
                // }
                // return result;
                throw new NotImplementedException();
            }
        }
    }
}
