// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Common.Enumerations;
using MSDF.DataChecker.Domain.Entities.Enumeration;
using MSDF.DataChecker.Domain.Entities.Metadata;
using MSDF.DataChecker.Domain.Extensions;

namespace MSDF.DataChecker.Domain
{
    public class DatabaseContext : DbContext
    {
        private readonly DatabaseEngine _databaseEngine;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, DatabaseEngine databaseEngine)
            : base(options)
        {
            _databaseEngine = databaseEngine;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<ContainerType> ContainerTypes { get; set; }

        public DbSet<DatabaseEngineType> DatabaseEngineTypes { get; set; }

        public DbSet<DiagnosticDataDisplayFormatType> DiagnosticDataDisplayFormatTypes { get; set; }

        public DbSet<ErrorSeverityLevelType> ErrorSeverityLevelTypes { get; set; }

        public DbSet<StatusType> StatusTypes { get; set; }

        public DbSet<Container> Containers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyDatabaseServerSpecificConventions(_databaseEngine);

            // apply all of the custom configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContainerEntityTypeConfiguration).Assembly);
        }
    }
}
