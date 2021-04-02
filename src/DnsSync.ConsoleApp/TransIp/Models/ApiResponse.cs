using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DnsSync.ConsoleApp.TransIp.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("domains")]
        public List<Domain> Domains { get; set; }
        
        [JsonPropertyName("dnsEntries")]
        public List<DnsEntry> DnsEntries { get; set; }
    }
}