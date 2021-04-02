using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.TransIp.Models;

namespace DnsSync.ConsoleApp.TransIp
{
    internal class TransIpApiClient : ITransIpApiClient
    {
        private readonly HttpClient _client;

        public TransIpApiClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://api.transip.nl/v6/");
        }

        public async Task<IList<Domain>> GetDomains()
        {
            var response = await _client.GetFromJsonAsync<ApiResponse>("domains");
            return response?.Domains ?? new List<Domain>();
        }
        
        public async Task<IList<DnsEntry>> GetDnsEntries(string domain)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse>($"domains/{domain}/dns");
            return response?.DnsEntries ?? new List<DnsEntry>();
        }
    }
}