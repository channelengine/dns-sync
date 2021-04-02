using System.Threading.Tasks;
using DnsSync.ConsoleApp.TransIp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp
{
    internal class DnsSyncService : IDnsSyncService
    {
        private readonly ILogger<DnsSyncService> _logger;
        private readonly ITransIpApiClient _transIpApiClient;

        public DnsSyncService(ILogger<DnsSyncService> logger,
            ITransIpApiClient transIpApiClient
        )
        {
            _logger = logger;
            _transIpApiClient = transIpApiClient;
        }

        public async Task Sync()
        {
            var token = await _transIpApiClient.GetAccessToken();
        }
    }
}