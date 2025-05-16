using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace NovoNordisk.Configuration.AzureBlob;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds a JSON configuration source from an Azure Blob to the configuration builder.
    /// </summary>
    /// <param name="builder">The configuration builder to add to.</param>
    /// <param name="blobUrl">URL to the Azure Blob.</param>
    /// <param name="reloadOnChange">Whether to reload configuration, if blob changes.</param>
    /// <param name="pollingInterval">The interval to check for changes.</param>
    /// <param name="tokenCredential">Credentials to access the blob, if not public.</param>
    /// <param name="exceptionHandler">Action to be called when an exception occurs during reload.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddJsonBlob(this IConfigurationBuilder builder, Uri blobUrl, bool reloadOnChange = false, TimeSpan? pollingInterval = null, TokenCredential? tokenCredential = null, Action<Exception>? exceptionHandler = null)
    {
        return builder.Add(new BlobJsonConfigurationSource
        {
            BlobUrl = blobUrl,
            ReloadOnChange = reloadOnChange,
            PollingInterval = pollingInterval ?? TimeSpan.FromSeconds(30),
            Credential = tokenCredential,
            ExceptionHandler = exceptionHandler
        });
    }
}