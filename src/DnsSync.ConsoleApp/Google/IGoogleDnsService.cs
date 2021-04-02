using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Dns.v1.Data;

namespace DnsSync.ConsoleApp.Google
{
    public interface IGoogleDnsService
    {
        Task<IList<ManagedZone>> GetManagedZones();
    }
}