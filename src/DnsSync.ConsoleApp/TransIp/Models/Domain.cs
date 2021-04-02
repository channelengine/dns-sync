using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DnsSync.ConsoleApp.TransIp.Models
{
    public class Domain
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("authCode")]
        public string AuthCode { get; set; }

        [JsonPropertyName("isTransferLocked")]
        public bool IsTransferLocked { get; set; }

        [JsonPropertyName("registrationDate")]
        public string RegistrationDate { get; set; }

        [JsonPropertyName("renewalDate")]
        public string RenewalDate { get; set; }

        [JsonPropertyName("isWhitelabel")]
        public bool IsWhitelabel { get; set; }

        [JsonPropertyName("cancellationDate")]
        public string CancellationDate { get; set; }

        [JsonPropertyName("cancellationStatus")]
        public string CancellationStatus { get; set; }

        [JsonPropertyName("isDnsOnly")]
        public bool IsDnsOnly { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}