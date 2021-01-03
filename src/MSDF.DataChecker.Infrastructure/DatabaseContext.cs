﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Domain.Entities;

namespace MSDF.DataChecker.Domain
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Container> Containers { get; set; }

        public DbSet<ContainerType> ContainerTypes { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<RuleExecutionLog> RuleExecutionLogs { get; set; }

        public DbSet<DatabaseEnvironment> DatabaseEnvironments { get; set; }

        public DbSet<UserParam> UserParams { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<TagEntity> TagEntities { get; set; }

        public DbSet<EdFiRuleExecutionLogDetail> EdFiRuleExecutionLogDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seeding Initial Data

            modelBuilder.Entity<ContainerType>().HasData(
                new ContainerType
                {
                    Id = 1,
                    Name = "Collection"
                });

            modelBuilder.Entity<ContainerType>().HasData(
                new ContainerType
                {
                    Id = 2,
                    Name = "Folder"
                });
        }
    }
}
