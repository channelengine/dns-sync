using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using DnsSync.ConsoleApp.TransIp.Models;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp.TransIp.Auth
{
    public class TransIpAuthClient : ITransIpAuthClient
    {
        private readonly HttpClient _client;
        private readonly IOptionsMonitor<TransIpApiConfiguration> _config;

        public TransIpAuthClient(HttpClient client, IOptionsMonitor<TransIpApiConfiguration> config)
        {
            _client = client;
            _config = config;
            _client.BaseAddress = new Uri("https://api.transip.nl/v6/");
        }

        public async Task<string> GetToken()
        {
            var config = _config.CurrentValue;
            
            if (string.IsNullOrEmpty(config.PrivateKey)) throw new Exception("Invalid private key");

            var nonce = Guid.NewGuid().ToString("N");
            var expiry = TimeSpan.FromMinutes(15);

            var body = new AuthRequest()
            {
                Login = config.Username,
                Label = $"dns-sync-{nonce}",
                ExpirationTime = $"{expiry.TotalSeconds} seconds",
                ReadOnly = true,
                GlobalKey = false,
                Nonce = nonce
            };

            using var ms = new MemoryStream();
            
            var content = JsonContent.Create(body);
            await content.CopyToAsync(ms);
            ms.Position = 0;
                
            var signature = GetSignature(config.PrivateKey, ms);
            var request = new HttpRequestMessage(HttpMethod.Post, "auth") { Content = content };
            request.Headers.Add("Signature", signature);
                
            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result == null) throw new Exception("Invalid token payload");
            
            return result.Token;
        }

        public static string GetSignature(string privateKey, Stream body)
        {
            using var rsa = RSA.Create();
            
            rsa.ImportFromPem(privateKey);
            var signature = rsa.SignData(body, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }
    }
}