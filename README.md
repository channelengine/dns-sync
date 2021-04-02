# TransIp to Google Cloud DNS sync

This tool allows for easily synchronizing DNS records from TransIp to Google Cloud DNS

## Configuration

Modify `src/DnsSync.ConsoleApp/appsettings.json` or enable user secrets to use the correct credentials.

```
{
    "TransIpApi": {
        "Username": "myusername",
        "PrivateKey": "-----BEGIN PRIVATE KEY-----\nxxxxxxxx\n-----END PRIVATE KEY-----\n"
    },
    "GoogleApi": {
        "ProjectId": "googlecloudprojectid",
        "ClientId": "123456789000",
        "PrivateKey": "-----BEGIN PRIVATE KEY-----\nxxxxxxxx\n-----END PRIVATE KEY-----\n"
    }
}
```
