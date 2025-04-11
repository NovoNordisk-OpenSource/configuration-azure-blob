# Azure Blob Storage Configuration Provider
This is a configuration provider for the built-in [Configuration API](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration), for reading JSON configuration files from Azure Blob Storage.

Features:
- Authentication via Azure token credential
- Reloadable config, with adjustable cadence
- Invoke `Action` on exception

## Usage
To get started, all you need is the URL for the specific blob you want to load.

```csharp
string blobUrl = "...";
builder.Configuration.AddJsonBlob(blobUrl);
```

This requires the blob to be public. If you need to use Azure credentials to access the blob, these can be passed to the `AddJsonBlob` method.

```csharp
string blobUrl = "...";
builder.Configuration.AddJsonBlob(blobUrl, tokenCredential: new DefaultAzureCredential());
```

By default, the blob is read on first use, and kept in the application. If the file is changed, and you want the application to react/update when the file changes, you need to enable reload.

```csharp
string blobUrl = "...";
builder.Configuration.AddJsonBlob(blobUrl, reloadOnChange: true);
```

This will check the ETag on the file every 30 seconds. If you want to use a different cadence, you can specify a timespan for the delay between checks.

```csharp
string blobUrl = "...";
builder.Configuration.AddJsonBlob(blobUrl, reloadOnChange: true, pollingInterval: TimeSpan.FromMinutes(5));
```



// TODO: Exception handling