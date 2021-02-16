// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Configuration;
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

                Log.Information("Starting host");
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
    }
}
