# Scanner

The scanner searches a webpage and any linked browserconfig and manifest files for possible favicons.  The icons are *possible favicons* and are not verified or validated by the scanner.

If only one specific icon is desired, then the fetcher may be the best choice instead.  See Fetcher.


## Properties


### Source

The `ISource` being used to download resources.

```csharp
ISource Source { get; }
```

While this property is readonly, it may be set in the constructor.


## Constructors


### Default Constructor

Constructs a scanner that uses an `HttpSource` to download resources.

```csharp
Scanner()
```

### Construct with ISource

Constructs a scanner that uses the specified `ISource` to download resources.

```csharp
Scanner(ISource source)
```

This constructor, along with an `HttpSource` is useful for specifying caching rules.  For example:

```csharp
var source = new HttpSource() {
    CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable)
};
var scanner = new Scanner(source);
```


## Methods


### Scan

Scans a uri for possible favicons.

```csharp
IEnumerable<ScanResult> Scan(Uri uri)
```

The uri does not have to be a valid webpage.  In that case, only "/favicon.ico" is in the returned enumeration.

The return value can be used to iterate over the results, which are unverified possible favicons.

No request is made until the first item in the enumeration is accessed.  Further requests are likewise done on demand.  This behavior may be utilized to save on bandwidth by not iterating over all results if the perfect one is found early.

```csharp
foreach (var result in new Scanner().Scan(uri)) {
    if (_IsTheOneWeWant(result)) {
        _DoSomething(result);
        break;
    }
}
```

The above code will normally make only one request.
