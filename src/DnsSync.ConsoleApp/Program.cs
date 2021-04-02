using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using DnsSync.ConsoleApp.Google;
using DnsSync.ConsoleApp.TransIp;
using DnsSync.ConsoleApp.TransIp.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnsSync.ConsoleApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var service = host.Services.GetRequiredService<IDnsSyncService>();
            await service.Sync();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.Configure<GoogleApiConfiguration>(context.Configuration.GetSection(GoogleApiConfiguration.Section));
                services.Configure<TransIpApiConfiguration>(context.Configuration.GetSection(TransIpApiConfiguration.Section));
                
                services.AddTransient<TransIpAuthHandler>();
                services.AddHttpClient<ITransIpAuthClient, TransIpAuthClient>();
                services.AddHttpClient<ITransIpApiClient, TransIpApiClient>()
                    .AddHttpMessageHandler<TransIpAuthHandler>();
                
                services.AddSingleton<IGoogleDnsService, GoogleDnsService>();
                services.AddSingleton<IDnsSyncService, DnsSyncService>();
            });
    }
}
