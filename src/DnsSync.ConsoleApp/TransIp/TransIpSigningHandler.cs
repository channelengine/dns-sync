using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using Microsoft.Extensions.Options;

namespace DnsSync.ConsoleApp.TransIp
{
    public class TransIpSigningHandler : DelegatingHandler
    {
        private readonly IOptionsMonitor<TransIpApiConfiguration> _config;

        public TransIpSigningHandler(IOptionsMonitor<TransIpApiConfiguration> config)
        {
            _config = config;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await SignRequest(request);
            
            return await base.SendAsync(request, cancellationToken);
        }

        public async Task SignRequest(HttpRequestMessage request)
        {
            var config = _config.CurrentValue;

            if (string.IsNullOrEmpty(config.PrivateKey)) throw new Exception("Invalid private key");

            if (request.Content == null) return;

            using(var sha = SHA512.Create())
            using(var rsa = RSA.Create())
            using(var ms = new MemoryStream())
            {
                await request.Content.LoadIntoBufferAsync();
                
                await request.Content.CopyToAsync(ms);
                ms.Position = 0;

                var hash = await sha.ComputeHashAsync(ms);
                
                rsa.ImportFromPem(config.PrivateKey);
                var signature = rsa.SignHash(hash, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                request.Headers.Add("Signature", Convert.ToBase64String(signature));
            }
        }
    }
}