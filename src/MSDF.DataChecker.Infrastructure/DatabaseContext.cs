// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Common.Enumerations;
using MSDF.DataChecker.Domain.Extensions;

namespace MSDF.DataChecker.Domain
{
    public class DatabaseContext : DbContext
    {
        private readonly DatabaseEngine _databaseEngine;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, DatabaseEngine databaseEngine): base(options)
        {
            _databaseEngine = databaseEngine;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyDatabaseServerSpecificConventions(_databaseEngine);
        }
    }
}
