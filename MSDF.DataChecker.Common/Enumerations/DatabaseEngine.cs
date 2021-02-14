// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MSDF.DataChecker.Common.Enumerations
{
    public class DatabaseEngine : Enumeration<DatabaseEngine, string>
    {
        public static readonly DatabaseEngine SqlServer = new DatabaseEngine( DataCheckerConstants.SqlServer, "SQL Server");
        public static readonly DatabaseEngine PostgreSQL = new DatabaseEngine(DataCheckerConstants.PostgreSQL, "PostgreSQL");

        public DatabaseEngine(string value, string displayName)
            : base(value, displayName)
        {
        }

        public static DatabaseEngine TryParseEngine(string value)
        {
            if (TryParse(x => x.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase), out DatabaseEngine engine))
            {
                return engine;
            }

            throw new NotSupportedException(
                $"Not supported DatabaseEngine \"{value}\". Supported engines: {DataCheckerConstants.SqlServer}, and {DataCheckerConstants.PostgreSQL}.");
        }
    }
}
