using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace NovoNordisk.Configuration.AzureBlob;

public class BlobJsonConfigurationSource : IConfigurationSource
{
    public required string BlobUrl { get; init; }
    public bool ReloadOnChange { get; init; } = false;
    public TimeSpan PollingInterval { get; init; } = TimeSpan.FromSeconds(30);
    public Action<Exception>? ExceptionHandler { get; init; }
    public TokenCredential? Credential { get; init; }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new BlobJsonConfigurationProvider(this);
    }
}
