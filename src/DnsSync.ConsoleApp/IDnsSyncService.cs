using System.Threading.Tasks;

namespace DnsSync.ConsoleApp
{
    public interface IDnsSyncService
    {
        Task Sync();
    }
}