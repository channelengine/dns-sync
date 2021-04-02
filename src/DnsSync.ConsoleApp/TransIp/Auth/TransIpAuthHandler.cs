using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DnsSync.ConsoleApp.TransIp.Auth
{
    public class TransIpAuthHandler : DelegatingHandler
    {
        private readonly ITransIpAuthClient _authClient;

        public TransIpAuthHandler(ITransIpAuthClient authClient)
        {
            _authClient = authClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authClient.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}