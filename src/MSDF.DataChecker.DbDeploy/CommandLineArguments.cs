// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using CommandLine;

namespace MSDF.DataChecker.DbDeploy
{
    public class CommandLineArguments
    {
        [Option("engine", Required = true, HelpText = "Database Engine to run against (postgreSql or sqlServer")]
        public string DatabaseEngine { get; set; }

        [Option('c', "connectionString", Required=true, HelpText = "Connection String to deploy to.")]
        public string  ConnectionString { get; set; }

        public static IDictionary<string, string> SwitchingMapping()
            => new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"-c", "ConnectionStrings:DataCheckerStore"},
                {"--connectionString", "ConnectionStrings:DataCheckerStore"},
                {"--engine", "DatabaseEngine"}
            };
    }
}
