// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MSDF.DataChecker.Common.Enumerations;

namespace MSDF.DataChecker.Domain.Extensions
{
    public static class DatabaseModelBuilderExtensions
    {
        public static void ApplyDatabaseServerSpecificConventions(this ModelBuilder modelBuilder, DatabaseEngine databaseEngine)
        {
            if (databaseEngine.Equals(DatabaseEngine.SqlServer))
            {
                return;
            }

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToLowerInvariant());

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema())).ToLowerInvariant());

                foreach (var key in entity.GetKeys())
                    key.SetName(key.GetName().ToLowerInvariant());

                foreach (var key in entity.GetForeignKeys())
                    key.SetConstraintName(key.GetConstraintName().ToLowerInvariant());

                foreach (var index in entity.GetIndexes())
                    index.SetDatabaseName(index.GetDatabaseName().ToLowerInvariant());
            }
        }
    }
}
