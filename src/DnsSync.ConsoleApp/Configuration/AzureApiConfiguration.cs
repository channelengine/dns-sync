namespace DnsSync.ConsoleApp.Configuration
{
    public class AzureApiConfiguration
    {
        public const string Section = "AzureApi";
        
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
    }
}