// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Common.Enumerations;
using MSDF.DataChecker.Domain;

namespace MSDF.DataChecker.Tests.MappingTests
{
    public class SqliteDatabaseContext : DatabaseContext
    {
        public DbSet<TestTable> TestTables { get; set; }

        public SqliteDatabaseContext(DbContextOptions<DatabaseContext> options, DatabaseEngine databaseEngine)
            : base(options, databaseEngine) { }
    }
}
