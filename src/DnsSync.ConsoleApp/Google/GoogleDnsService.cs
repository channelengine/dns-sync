using System.Collections.Generic;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Dns.v1;
using Google.Apis.Dns.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp.Google
{
    public class GoogleDnsService : IGoogleDnsService
    {
        private DnsService _client;
        private string _projectId;

        public GoogleDnsService(IOptionsMonitor<GoogleApiConfiguration> config)
        {
            config.OnChange(c =>
            {
                _client = GetClient(c);
                _projectId = c.ProjectId;
            });
                
            _client = GetClient(config.CurrentValue);
            _projectId = config.CurrentValue.ProjectId;
        }

        public async Task<IList<ManagedZone>> GetManagedZones()
        {
            var request = _client.ManagedZones.List(_projectId);
            var response = await request.ExecuteAsync();
            return response.ManagedZones;
        }

        public async Task<ManagedZone> CreateManagedZone(ManagedZone zone)
        {
            var request = _client.ManagedZones.Create(zone, _projectId);
            var response = await request.ExecuteAsync();
            return response;
        }
        
        public async Task<IList<ResourceRecordSet>> GetRecords(string zoneName)
        {
            var request = _client.ResourceRecordSets.List(_projectId, zoneName);
            var response = await request.ExecuteAsync();
            return response.Rrsets;
        }

        public async Task<Change> CreateChange(string zoneName, Change change)
        {
            var request = _client.Changes.Create(change, _projectId, zoneName);
            var response = await request.ExecuteAsync();
            return response;
        }

        private static DnsService GetClient(GoogleApiConfiguration config)
        {
            return new(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(config),
                ApplicationName = "DnsSync",
            });
        }
        
        private static GoogleCredential GetCredential(GoogleApiConfiguration config)
        {
            var initializer = new ServiceAccountCredential.Initializer(config.ClientId);
            initializer.FromPrivateKey(config.PrivateKey);

            var serviceAccountCredential = new ServiceAccountCredential(initializer);
            var credential = GoogleCredential.FromServiceAccountCredential(serviceAccountCredential);
            
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }

            return credential;
        }
    }
}