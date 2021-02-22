// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MSDF.DataChecker.JobExecutorDaemon
{
    public class HangfireServer : IHostedService
    {
        private readonly ILogger<HangfireServer> _logger;
        private readonly int _workerCount;
        private BackgroundJobServer _server;

        public HangfireServer(ILogger<HangfireServer> logger, IConfiguration configuration)
        {
            _logger = logger;

            string connectionString = configuration.GetConnectionString("RulesExecutorStore");
            _workerCount = configuration.GetValue<int?>("JobExecutor:Processes") ?? 2;

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation Requested before starting Hangfire");
                return Task.FromCanceled(cancellationToken);
            }

            _logger.LogInformation("Starting Hangfire");

            _server = new BackgroundJobServer(new BackgroundJobServerOptions {WorkerCount = _workerCount});

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Hangfire");

            if (_server != null)
            {
                _server.SendStop();
                await _server.WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
