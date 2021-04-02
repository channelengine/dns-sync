using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DnsSync.ConsoleApp.TransIp.Models
{
    public class AuthRequest
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }

        [JsonPropertyName("read_only")]
        public bool? ReadOnly { get; set; }

        [JsonPropertyName("expiration_time")]
        public string ExpirationTime { get; set; }
        
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("global_key")]
        public bool? GlobalKey { get; set; }
    }
}