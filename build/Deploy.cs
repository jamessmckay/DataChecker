// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Diagnostics;
using Nuke.Common;

partial class Build
{
    Target Deploy
        => _ => _
            .DependsOn(CI)
            .DependsOn(DeployDatabase)
            .DependsOn(DeployTestDatabase)
            .Executes();

    Target DeployTestDatabase
        => _ => _
            .Executes(() =>
            {
                var databaseName = ResolvedDatabaseName + "_test";

                var args = $"-e {ResolvedDatabaseEngine} -c \"{CreateConnectionString(databaseName)}\"";

                Logger.Info($"{DbDeploy} {args}");

                var info = new ProcessStartInfo(DbDeploy, args)
                {
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                };

                RunDbDeploy(info);
            });

    Target DeployDatabase
        => _ => _
            .Executes(() =>
            {
                var args = $"-e {ResolvedDatabaseEngine} -c \"{CreateConnectionString(ResolvedDatabaseName)}\"";

                Logger.Info($"{DbDeploy} {args}");

                var info = new ProcessStartInfo(DbDeploy, args)
                {
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                };

                RunDbDeploy(info);
            });

    static void RunDbDeploy(ProcessStartInfo info)
    {
        using var process = new Process {StartInfo = info};

        process.OutputDataReceived += (sender, e) => { Logger.Info($"Output: {e.Data}"); };
        process.ErrorDataReceived += (sender, e) => { Logger.Error($"Error: {e.Data}"); };

        process.Start();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new ApplicationException("Failed to deploy database");
        }
    }

    string CreateConnectionString(string databaseName)
    {
        string template = !string.IsNullOrEmpty(DatabaseEngine)
            ? !string.IsNullOrEmpty(ConnectionStringTemplate)
                ? ConnectionStringTemplate
                : DatabaseEngine.Equals("postgresql", StringComparison.InvariantCultureIgnoreCase)
                    ? DefaultPostgresConnectionStringTemplate
                    : DefaultSqlServerConnectionStringTemplate
            : ConnectionStringTemplate ?? DefaultPostgresConnectionStringTemplate;

        return string.Format(template, databaseName);
    }

    string ResolvedDatabaseName =>  DatabaseName ?? "data_checker";

    string ResolvedDatabaseEngine => DatabaseEngine ?? "postgresql";

    string DbDeploy => DeployDirectory / "bin" / Configuration / "netcoreapp3.1" / "MSDF.Datachecker.DbDeploy";
}
