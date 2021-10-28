using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.FeeShareEngine.Writer.Services;

namespace Service.FeeShareEngine.Writer
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly ServiceBusLifeTime _client;
        private readonly FeeShareWriter _feeShareWriter;
        private readonly FeePaymentWriter _feePaymentWriter;
        private readonly MyNoSqlClientLifeTime _myNoSqlClientLifeTime;
        private SharesPaymentService _sharesPaymentService;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, ServiceBusLifeTime client, FeeShareWriter feeShareWriter, FeePaymentWriter feePaymentWriter, MyNoSqlClientLifeTime myNoSqlClientLifeTime, SharesPaymentService sharesPaymentService)
            : base(appLifetime)
        {
            _logger = logger;
            _client = client;
            _feeShareWriter = feeShareWriter;
            _feePaymentWriter = feePaymentWriter;
            _myNoSqlClientLifeTime = myNoSqlClientLifeTime;
            _sharesPaymentService = sharesPaymentService;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _client.Start();
            _feePaymentWriter.Start();
            _myNoSqlClientLifeTime.Start();
            _sharesPaymentService.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _client.Stop();
            _myNoSqlClientLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
