﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace MSDF.DataChecker.WebApi
{
    internal static class Program
    {
        private static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();

        public static async Task Main(string[] args)
        {
            IHost host = null;

            try
            {
                host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .UseSerilog(
                        (hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .Enrich.FromLogContext()
                                .WriteTo.Providers(Providers);
                        })
                    .ConfigureLogging(logger => logger.AddConsole())
                    .ConfigureWebHostDefaults(
                        webBuilder =>
                        {
                            webBuilder.ConfigureKestrel(serverOptions => serverOptions.AddServerHeader = false);

                            webBuilder.UseStartup<Startup>();
                        })
                    .Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                host?.Dispose();
                Log.CloseAndFlush();
            }
        }
    }
}
