using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace NovoNordisk.Configuration.AzureBlob;

public class BlobJsonConfigurationProvider : ConfigurationProvider, IDisposable, IAsyncDisposable
{
    private readonly BlobJsonConfigurationSource _source;
    private readonly Timer? _timer;
    private ETag? _etag;
    private int _reloadInProgress;

    public BlobJsonConfigurationProvider(BlobJsonConfigurationSource source)
    {
        _source = source;
        
        if (_source.ReloadOnChange)
        {
            _timer = new Timer(CheckForChanges, null, _source.PollingInterval, _source.PollingInterval);
        }
    }
    
    public override void Load()
    {
        LoadAsync().GetAwaiter().GetResult();
    }

    private async Task LoadAsync()
    {
        var client = GetBlobClient();
        using var stream = new MemoryStream();
        using var response = await client.DownloadToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);

        if (!response.IsError)
        {
            Data = JsonConfigurationFileParser.Parse(stream);
            _etag = response.Headers.ETag;
            
            OnReload();
        }
    }

    private async void CheckForChanges(object? state)
    {
        try
        {
            if (Interlocked.CompareExchange(ref _reloadInProgress, 1, 0) == 0)
            {
                var client = GetBlobClient();
                var props = await client.GetPropertiesAsync();
                if (props is null || !props.HasValue)
                {
                    throw new RequestFailedException("Failed to retrieve blob properties");
                }

                if (_etag != props.Value.ETag)
                {
                    await LoadAsync();
                }
            }
        }
        catch (Exception e)
        {
            _source.ExceptionHandler?.Invoke(e);
        }
        finally
        {
            Interlocked.CompareExchange(ref _reloadInProgress, 0, 1);
        }
    }
    
    private BlobClient GetBlobClient()
    {
        return new BlobClient(_source.BlobUrl, _source.Credential);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer != null) await _timer.DisposeAsync();
    }
}