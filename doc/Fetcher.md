# Fetcher

The fetcher scans a webpage for possible favicons.  It downloads and verifies the one that best matches specified constraints.

The fetcher minimizes bandwidth by using the load-on-demand feature of the scanner and prioritizing favicons to check.  If more than three requests are being made on average, then check the methods and parameters being used.


## Properties


### Source

The `ISource` being used to download resources.

```csharp
ISource Source { get; }
```

While this property is readonly, it may be set in the constructor.


## Constructors


### Default Constructor

Constructs a fetcher that uses an `HttpSource` to download resources.

```csharp
Fetcher()
```

### Construct with ISource

Constructs a fetcher that uses the specified `ISource` to download resources.

```csharp
Fetcher(ISource source)
```

This constructor, along with an `HttpSource` is useful for specifying caching rules.  For example:

```csharp
var source = new HttpSource() {
    CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable)
};
var fetcher = new Fetcher(source);
```


## Methods


### FetchClosest (recommended)

Scans a webpage and any linked resources for favicon references, returning the closest valid favicon to the specified size.

```csharp
Image FetchClosest(Uri uri, Size size)
```

If a valid favicon that meets the constraints cannot be found, then `null` is returned.  Otherwise, an image is returned, and it must be properly disposed when it is no longer needed.


### FetchExact

Scans a webpage and any linked resources for favicon references, returning the valid favicon that is exactly the specified size.

```csharp
Image FetchExact(Uri uri, Size size)
```

If a valid favicon that meets the constraints cannot be found, then `null` is returned.  Otherwise, an image is returned, and it must be properly disposed when it is no longer needed.


### Fetch

Scans a webpage and any linked resources for favicon references, returning the valid favicon that most closely matches the specified constraints.

```csharp
Image Fetch(Uri uri, FetchOptions options)
```

This is the most customizable method of fetching a favicon.  For the full options available, see FetchOptions.

If a valid favicon that meets the constraints cannot be found, then `null` is returned.  Otherwise, an image is returned, and it must be properly disposed when it is no longer needed.

