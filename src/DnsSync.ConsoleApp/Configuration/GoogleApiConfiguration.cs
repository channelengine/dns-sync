namespace DnsSync.ConsoleApp.Configuration
{
    public class GoogleApiConfiguration
    {
        public const string Section = "GoogleApi";
        
        public string ClientId { get; set; }
        public string PrivateKey { get; set; }
        public string ProjectId { get; set; }
    }
}