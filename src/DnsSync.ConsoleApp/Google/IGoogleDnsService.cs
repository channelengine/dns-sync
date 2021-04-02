using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Dns.v1.Data;

namespace DnsSync.ConsoleApp.Google
{
    public interface IGoogleDnsService
    {
        Task<IList<ManagedZone>> GetManagedZones();
        Task<ManagedZone> CreateManagedZone(ManagedZone zone);
        Task<IList<ResourceRecordSet>> GetRecords(string zoneName);
        Task<Change> CreateChange(string zoneName, Change change);
    }
}