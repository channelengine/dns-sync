using System.Threading.Tasks;
using DnsSync.ConsoleApp.Google;
using DnsSync.ConsoleApp.TransIp;
using Microsoft.Extensions.Logging;

namespace DnsSync.ConsoleApp
{
    internal class DnsSyncService : IDnsSyncService
    {
        private readonly ILogger<DnsSyncService> _logger;
        private readonly IGoogleDnsService _googleDnsService;
        private readonly ITransIpApiClient _transIpApiClient;

        public DnsSyncService(ILogger<DnsSyncService> logger,
            IGoogleDnsService googleDnsService,
            ITransIpApiClient transIpApiClient
        )
        {
            _logger = logger;
            _googleDnsService = googleDnsService;
            _transIpApiClient = transIpApiClient;
        }

        public async Task Sync()
        {
            var domains = await _transIpApiClient.GetDomains();
            var zones = await _googleDnsService.GetManagedZones();
        }
    }
}