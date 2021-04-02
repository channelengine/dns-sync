using System.Text.Json.Serialization;

namespace DnsSync.ConsoleApp.TransIp.Models
{
    public class DnsEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("expire")]
        public int Expire { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}