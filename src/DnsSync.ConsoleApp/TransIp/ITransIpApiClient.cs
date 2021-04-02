using System.Collections.Generic;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.TransIp.Models;

namespace DnsSync.ConsoleApp.TransIp
{
    public interface ITransIpApiClient
    {
        Task<IList<Domain>> GetDomains();
        Task<IList<DnsEntry>> GetDnsEntries(string domain);
    }
}