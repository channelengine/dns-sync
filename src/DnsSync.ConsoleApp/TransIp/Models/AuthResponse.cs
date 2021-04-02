using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DnsSync.ConsoleApp.TransIp.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}