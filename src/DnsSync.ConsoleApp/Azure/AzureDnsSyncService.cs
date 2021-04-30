using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.TransIp;

namespace DnsSync.ConsoleApp.Azure
{
    internal class AzureDnsSyncService : IDnsSyncService
    {
        private readonly IAzureDnsService _azureDnsService;
        private readonly ITransIpApiClient _transIpApiClient;

        public AzureDnsSyncService(
            IAzureDnsService azureDnsService,
            ITransIpApiClient transIpApiClient
        )
        {
            _azureDnsService = azureDnsService;
            _transIpApiClient = transIpApiClient;
        }

        public async Task Sync()
        {
            var transIpDomains = await _transIpApiClient.GetDomains();
        }
    }
}