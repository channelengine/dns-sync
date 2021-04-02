using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using DnsSync.ConsoleApp.TransIp.Models;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp.TransIp
{
    internal class TransIpApiClient : ITransIpApiClient
    {
        private readonly HttpClient _client;
        private readonly IOptionsMonitor<TransIpApiConfiguration> _config;

        private const string Label = "dns-sync";
        private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(15);

        public TransIpApiClient(HttpClient client, IOptionsMonitor<TransIpApiConfiguration> config)
        {
            _client = client;
            _config = config;
            _client.BaseAddress = new Uri("https://api.transip.nl/v6/");
        }

        public async Task<string> GetAccessToken()
        {
            var config = _config.CurrentValue;

            var body = new AuthRequest()
            {
                Login = config.Username,
                Label = Label,
                ExpirationTime = $"{ExpirationTime.TotalSeconds} seconds",
                ReadOnly = true,
                GlobalKey = false,
                Nonce = Guid.NewGuid().ToString("N")
            };
            
            var response = await _client.PostAsJsonAsync("auth", body);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result == null) throw new Exception("Invalid token payload");
            
            return result.Token;
        }
    }
}