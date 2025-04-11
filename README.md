# Azure Blob Storage Configuration Provider
This is a configuration provider for the built-in [Configuration API](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration), for reading JSON configuration files from Azure Blob Storage.

Features:
- Authentication via Azure token credential
- Reloadable config, with adjustable cadence
- Invoke `Action` on exception

## Installation
You can install the library via NuGet:

```bash
dotnet add package NovoNordisk.Configuration.AzureBlob
```

## Usage
To get started, all you need is the URL for the specific blob you want to load.

```csharp
var blobUrl = new Uri("...");
builder.Configuration.AddJsonBlob(blobUrl);
```

This requires the blob to be public. If you need to use Azure credentials to access the blob, these can be passed to the `AddJsonBlob` method.

```csharp
var blobUrl = new Uri("...");
builder.Configuration.AddJsonBlob(blobUrl, tokenCredential: new DefaultAzureCredential());
```

By default, the blob is read on first use, and kept in the application. If the file is changed, and you want the application to react/update when the file changes, you need to enable reload.

```csharp
var blobUrl = new Uri("...");
builder.Configuration.AddJsonBlob(blobUrl, reloadOnChange: true);
```

This will check the ETag on the file every 30 seconds. If you want to use a different cadence, you can specify a timespan for the delay between checks.

```csharp
var blobUrl = new Uri("...");
builder.Configuration.AddJsonBlob(blobUrl, reloadOnChange: true, pollingInterval: TimeSpan.FromMinutes(5));
```

If, for any reason, an error occurs during a reload, you can specify an exception handler to be invoked.

```csharp
var blobUrl = new Uri("...");
configurationBuilder.AddJsonBlob(blobUrl, exceptionHandler: exception =>
{
    Console.WriteLine(exception.Message);
});
```

## Contributing
Contributions are welcome! Please open an issue or submit a pull request.

## License
This project is licensed under the MIT License. See the LICENSE file for details.
