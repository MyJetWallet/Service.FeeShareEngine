﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;

namespace Service.FeeShareEngine
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly ServiceBusLifeTime _client;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, ServiceBusLifeTime client)
            : base(appLifetime)
        {
            _logger = logger;
            _client = client;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _client.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called."); 
            _client.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
