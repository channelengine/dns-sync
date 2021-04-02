using System;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using DnsSync.ConsoleApp.TransIp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnsSync.ConsoleApp
{
    class Program
    {
        public static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var service = host.Services.GetRequiredService<IDnsSyncService>();
            service.Sync();

            return host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.Configure<GoogleApiConfiguration>(context.Configuration.GetSection(GoogleApiConfiguration.Section));
                services.Configure<TransIpApiConfiguration>(context.Configuration.GetSection(TransIpApiConfiguration.Section));

                services.AddTransient<TransIpSigningHandler>();
                services.AddHttpClient<ITransIpApiClient, TransIpApiClient>()
                    .AddHttpMessageHandler<TransIpSigningHandler>();
                
                services.AddSingleton<IDnsSyncService, DnsSyncService>();
            });
    }
}
