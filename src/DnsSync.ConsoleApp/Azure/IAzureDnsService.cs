using System.Threading.Tasks;
using Microsoft.Azure.Management.Dns.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace DnsSync.ConsoleApp.Azure
{
    public interface IAzureDnsService
    {
        Task<IPagedCollection<IDnsZone>> GetZones();
    }
}