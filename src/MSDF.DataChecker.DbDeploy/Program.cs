// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSDF.DataChecker.Common.Enumerations;
using MSDF.DataChecker.DbDeploy.DatabaseCreators;
using MSDF.DataChecker.DbDeploy.UpgradeEngineBuilderFactories;
using Serilog;
using Serilog.Events;

namespace MSDF.DataChecker.DbDeploy
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                CommandLineArguments commandLineArguments = null;

                var parser = new Parser(
                        config =>
                        {
                            config.CaseInsensitiveEnumValues = true;
                            config.CaseSensitive = false;
                            config.HelpWriter = Console.Out;
                            config.IgnoreUnknownArguments = true;
                        })
                    .ParseArguments<CommandLineArguments>(args)
                    .WithParsed(args => commandLineArguments = args)
                    .WithNotParsed(
                        errs =>
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid options were entered.");

                            Environment.ExitCode = 1;
                            Environment.Exit(Environment.ExitCode);
                        });

                var configRoot = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets<Program>()
                    .AddCommandLine(args, CommandLineArguments.SwitchingMapping())
                    .Build();

                var serviceProvider = CreateServiceProvider(new ServiceCollection(), configRoot);

                var databaseCreator = serviceProvider.GetService<IDatabaseCreator>();

                databaseCreator.EnsureDatabaseIsCreated();

                foreach (var upgradeEngine in serviceProvider.GetService<IUpgradeEngineFactory>().Create())
                {
                    var result = upgradeEngine.PerformUpgrade();

                    if (!result.Successful)
                    {
                        throw result.Error;
                    }
                }

                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                Environment.ExitCode = 1;
            }
            finally
            {
                Log.CloseAndFlush();

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }
            }

            return Environment.ExitCode;
        }

        private static IServiceProvider CreateServiceProvider(IServiceCollection services, IConfiguration configuration)
        {
            var databaseEngine = DatabaseEngine.TryParseEngine(configuration.GetValue<string>("DatabaseEngine"));

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .AddSingleton(configuration)
                .AddSingleton(databaseEngine);

            if (databaseEngine.Equals(DatabaseEngine.PostgreSQL))
            {
                services
                    .AddSingleton<IDatabaseCreator, PostgresDatabaseCreator>()
                    .AddSingleton<IUpgradeEngineFactory, PostgresUpgradeEngineFactory>();
            }
            else
            {
                services
                    .AddSingleton<IDatabaseCreator, SqlServerDatabaseCreator>()
                    .AddSingleton<IUpgradeEngineFactory, SqlSeverUpgradeEngineFactory>();
            }

            return services.BuildServiceProvider();
        }
    }
}
