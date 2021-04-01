using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp
{
    internal class DnsSyncService : IDnsSyncService
    {
        private readonly ILogger<DnsSyncService> _logger;

        public DnsSyncService(
            ILogger<DnsSyncService> logger)
        {
            _logger = logger;
        }

        public async Task Sync()
        {
            return;
        }
    }
}