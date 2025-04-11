using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace NovoNordisk.Configuration.AzureBlob;

public static class ConfigurationBuilderExtensions
{
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