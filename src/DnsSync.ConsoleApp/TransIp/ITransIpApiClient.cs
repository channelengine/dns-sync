using System.Threading.Tasks;

namespace DnsSync.ConsoleApp.TransIp
{
    public interface ITransIpApiClient
    {
        Task<string> GetAccessToken();
    }
}