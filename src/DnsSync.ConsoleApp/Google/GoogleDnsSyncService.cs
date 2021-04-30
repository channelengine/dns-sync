using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Google;
using DnsSync.ConsoleApp.TransIp;
using Google.Apis.Dns.v1.Data;

namespace DnsSync.ConsoleApp.Google
{
    internal class GoogleDnsSyncService : IDnsSyncService
    {
        private readonly IGoogleDnsService _googleDnsService;
        private readonly ITransIpApiClient _transIpApiClient;

        public GoogleDnsSyncService(
            IGoogleDnsService googleDnsService,
            ITransIpApiClient transIpApiClient
        )
        {
            _googleDnsService = googleDnsService;
            _transIpApiClient = transIpApiClient;
        }

        public async Task Sync()
        {
            var transIpDomains = await _transIpApiClient.GetDomains();
            var googleZones = (await _googleDnsService.GetManagedZones())
                .ToDictionary(z => z.Name);
            
            foreach (var transIpDomain in transIpDomains)
            {
                var googleZoneName = transIpDomain.Name.Replace(".", "-");
                var googleZoneDnsName = $"{transIpDomain.Name}.";
                
                // Check if the zone already exists
                if (!googleZones.TryGetValue(googleZoneName, out var zone))
                {
                    // Create zone
                    zone = new ManagedZone()
                    {
                        DnsName = googleZoneDnsName,
                        Name = googleZoneName,
                        Description = transIpDomain.Name,
                        DnssecConfig = new ManagedZoneDnsSecConfig()
                        {
                            State = "on"
                        }
                    };

                    await _googleDnsService.CreateManagedZone(zone);
                }

                var transIpRecords = await _transIpApiClient.GetDnsEntries(transIpDomain.Name);
                var googleRecords = (await _googleDnsService.GetRecords(googleZoneName))
                    .ToDictionary(r => (r.Name, r.Type));
                
                var googleRecordAdditions = new Dictionary<(string, string), ResourceRecordSet>();
                var googleRecordDeletions = new Dictionary<(string, string), ResourceRecordSet>();
                
                foreach (var transIpRecord in transIpRecords)
                {
                    // Convert @ record to explicit records because google DNS does not support using @
                    var googleRecordName = transIpRecord.Name.Replace("@", googleZoneDnsName);
                    var googleRecordContent = transIpRecord.Content.Replace("@", googleZoneDnsName);
                    
                    if (transIpRecord.Type == "TXT")
                    {
                        // Remove whitespace in DKIM, DMARC
                        if(googleRecordContent.StartsWith("v="))
                        {
                            googleRecordContent = Regex.Replace(googleRecordContent, @";(\s+)", ";");
                        }
                        
                        // Quote strings
                        // See: https://www.mailhardener.com/blog/how-to-enter-txt-values-in-google-cloud-dns
                        googleRecordContent = $"\"{googleRecordContent}\"";
                    }
                    
                    // Always use full names
                    if (!googleRecordName.EndsWith(".")) googleRecordName += ".";
                    if (!googleRecordName.EndsWith(googleZoneDnsName)) googleRecordName += googleZoneDnsName;

                    // Check if the record already exists and was not yet processed for deletion
                    if(googleRecords.TryGetValue((googleRecordName, transIpRecord.Type), out var existingGoogleRecord) &&
                       !googleRecordDeletions.ContainsKey((googleRecordName, transIpRecord.Type)))
                    {
                        // Updates are not supported in the changes endpoint.
                        // Existing record should be deleted and recreated
                        // See: https://stackoverflow.com/questions/41818578/update-overwrite-dns-record-google-cloud
                        googleRecordDeletions.Add((googleRecordName, transIpRecord.Type), existingGoogleRecord);
                    }
                    
                    // Check if the record has already been processed for addition
                    if (googleRecordAdditions.TryGetValue((googleRecordName, transIpRecord.Type), out var googleRecord))
                    {
                        // Add content of records with the same name and type to the processed record
                        if(!googleRecord.Rrdatas.Contains(googleRecordContent))
                            googleRecord.Rrdatas.Add(googleRecordContent);
                    }
                    else
                    {
                        googleRecord = new ResourceRecordSet
                        {
                            Name = googleRecordName,
                            Type = transIpRecord.Type,
                            Ttl = transIpRecord.Expire,
                            Rrdatas = new List<string> { googleRecordContent }
                        };
                        
                        googleRecordAdditions.Add((googleRecordName, transIpRecord.Type), googleRecord);
                    }
                }

                var googleRecordChange = new Change()
                {
                    Additions = googleRecordAdditions.Values.ToList(),
                    Deletions = googleRecordDeletions.Values.ToList()
                };
                
                await _googleDnsService.CreateChange(googleZoneName, googleRecordChange);
            }
        }
    }
}