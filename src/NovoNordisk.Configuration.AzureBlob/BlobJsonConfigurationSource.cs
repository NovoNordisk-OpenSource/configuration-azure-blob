using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace NovoNordisk.Configuration.AzureBlob;

/// <summary>
/// Configuration source for Azure Blob JSON files.
/// </summary>
public class BlobJsonConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// URL for a Azure Blob JSON file.
    /// </summary>
    public required Uri BlobUrl { get; init; }
    
    /// <summary>
    /// Determines whether to check for file updates periodically.
    /// </summary>
    public bool ReloadOnChange { get; init; } = false;
    
    /// <summary>
    /// The interval at which the blob is checked for changes. 
    /// This is only used if <see cref="ReloadOnChange"/> is set to <c>true</c>.
    /// </summary>
    public TimeSpan PollingInterval { get; init; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Callback for when any exception occurs during reload.
    /// </summary>
    public Action<Exception>? ExceptionHandler { get; init; }
    
    /// <summary>
    /// Credentials for accessing Azure Blob Storage. Not needed if public.
    /// </summary>
    public TokenCredential? Credential { get; init; }
    
    /// <summary>
    /// Builds the <see cref="T:NovoNordisk.Configuration.BlobJsonConfigurationProvider" /> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
    /// <returns>An <see cref="T:NovoNordisk.Configuration.BlobJsonConfigurationProvider" /></returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new BlobJsonConfigurationProvider(this);
    }
}
