using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using Microsoft.Azure.Management.Dns.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp.Azure
{
    public class AzureDnsService : IAzureDnsService
    {
        private IAzure _client;

        public AzureDnsService(IOptionsMonitor<AzureApiConfiguration> config)
        {
            config.OnChange(c =>
            {
                _client = GetClient(c);
            });
                
            _client = GetClient(config.CurrentValue);
        }
        
        public Task<IPagedCollection<IDnsZone>> GetZones()
        {
            return _client.DnsZones.ListAsync();
        }

        private IAzure GetClient(AzureApiConfiguration config)
        {
            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                config.ClientId, config.Secret, config.TenantId, AzureEnvironment.AzureGlobalCloud);

            return Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();
        }
    }
}