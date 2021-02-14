// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MSDF.DataChecker.JobExecutorDaemon
{
    internal static class Program
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            Console.CancelKeyPress += (o, e) =>
            {
                Log.Warning("Ctrl-C Pressed. Stopping all threads.");
                _cancellationTokenSource.Cancel();
                e.Cancel = true;
            };

            Log.Debug("Starting JobExecutor Daemon");
            var cancellationToken = _cancellationTokenSource.Token;

            await host.RunAsync(cancellationToken);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(hostContext.Configuration)
                            .CreateLogger();

                        services.AddHostedService<HangfireServer>();
                    });
    }
}
