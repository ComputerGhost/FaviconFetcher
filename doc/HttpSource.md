# HttpSource

The default source used by the scanner and fetcher to download resources.  It may be customized and passed to their respective constructors.

HttpSource implements the `ISource` interface.


## Properties


### CachePolicy

Specifies how caching of resources should be done.

```csharp
RequestCachePolicy CachePolicy { get; set; }
```

The default value is `WebRequest.DefaultCachePolicy`.  For other options, see Microsoft's documentation on `RequestCachePolicy`.


### UserAgent

The user agent string to send along with all HTTP requests.

```csharp
string UserAgent { get; set; }
```

The default value is the FaviconFetcher's user agent string.


## Methods

While `DownloadText` and `DownloadImages` are public methods, they are not to be used outside of the FaviconFetcher library.  They are public because it is required by the `ISource` interface.
