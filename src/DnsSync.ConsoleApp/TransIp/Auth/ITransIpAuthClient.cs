using System.IO;
using System.Threading.Tasks;

namespace DnsSync.ConsoleApp.TransIp.Auth
{
    public interface ITransIpAuthClient
    {
        Task<string> GetToken();
    }
}