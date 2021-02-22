// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MSDF.DataChecker.DbDeploy
{
    public static class DeployConventions
    {
        public const string DataDirectory = "Data";
        public const string JournalTable = "DeployJournal";
        public const string MsSql = "MsSql";
        public const string PgSql = "PgSql";
        public const string ScriptsDirectory = "MSDF.DataChecker.DbDeploy.Scripts";
        public const string StructureDirectory = "Structure";

        public static class SqlServer
        {
            public const string DefaultSchema = "dbo";
        }

        public static class Postgres
        {
            public const string DefaultSchema = "public";
        }
    }
}
