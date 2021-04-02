namespace DnsSync.ConsoleApp.Configuration
{
    public class TransIpApiConfiguration
    {
        public const string Section = "TransIpApi";
        
        public string Username { get; set; }
        public string PrivateKey { get; set; }
    }
}