# Favicon Fetcher

FaviconFetcher will be released on November 30th, 2018.  You may use it in the meantime, but features and optimizations are still in progress.

FaviconFetcher is a library for scanning a webpage for favicons and downloading them.

### Features

 * Supports manifest.json and browserconfig.xml, if linked from the HTML. (not yet complete)
 * On-demand downloading to save bandwidth.
 * Cache shared with other applications (off by default).
 * Easy to use.
 
### Items left to do

 * Code in manifest.json and browserconfig.xml support.
 * Unit tests for Fetcher.
 * More readable unit tests.
 * Setup Nuget package.


## Installation

tbd.  Eventually, I hope to have this set up as a nuget package.


## How to Use

Common uses are listed below.  The full documentation exists in the public classes and the Object Browser.

### Scanner - Get a List of Favicons

The scanner parses a webpage, looking for references to favicons.

```
var scanner = new Scanner();
foreach (var result in scanner.Scan(uri))
{
    var expectedSize = result.ExpectedSize;
    var absoluteUri = result.Location;
    // do something with it.
}
```

The scanner downloads resources as needed, so breaking out of the loop early will save bandwidth.

#### Order of Favicons Returned

The HTML is first downloaded and parsed.  The order of favicon links returned are:

 * Those in `<link>` tags.
 * Those in browserconfig.xml and manifest.json, if referenced in the HTML. (not yet complete)
 * /favicon.ico.

If the webpage cannot be downloaded for parsing, then just the old default /favicon.ico is returned.

#### Watch out!

 * The favicons are not downloaded and verified, so they may not exist or may be invalid.
 * Duplicates may be returned.


### Fetcher - Find the Best Match

The scanner is great, but those favicons need downloaded, parsed, and verified.  The fetcher does that and returns the best match.

```
var fetcher = new Fetcher();
var image = fetcher.FetchClosest(uri, new Size(16, 16));
```

Pretty simple, right?  The fetcher has several methods to download the perfect favicon.  The one below is the most configurable:

```
var fetcher = new Fetcher();
var image = fetcher.Fetch(uri, new FetchOptions
{
    MinimumSize = minSize,
    MaximumSize = maxSize,
    PerfectSize = perfectSize,
    RequireSquare = true
});
```

#### Optimizing and Efficiency

Like the scanner, the fetcher downloads resources as needed.  The usual case is two HTTP requests, but the worst case is all of the resources being downloaded.  Passing the correct parameters is important.

Prefer methods that take a `PerfectSize` parameter, such as `FetchClosest` or `Fetch`.  When the perfect size is found, it doesn't bother downloading and processing the rest of them.


### HttpSource - Caching

Both `Scanner` and `Fetcher` have a constructor that takes an `ISource`, which controls how resources are downloaded.  `HttpSource` is the recommended one to use, and it supports caching that is shared with other applications.

By default, caching is turned off.  Turn it on by passing an `HttpSource` with `CachePolicy` set.

```
var source = new HttpSource() {
    CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
}
var fetcher = new Fetcher(source);
var image = fetcher.FetchClosest(uri, new Size(16, 16));
```

Or, you can just do this to use the cache for all requests:

```
WebRequest.DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
var fetcher = new Fetcher();
var image = fetcher.FetchClosest(uri, new Size(16, 16));
```


## Bug Reports

Please use the Issues tab on our GitHub page to file bug reports. Or, you may contact me directly if you know my contact information.


## Credits

### External libraries:
 * FaviconFetcher uses [ConcurrentPriorityQueue by dshulepov](https://github.com/dshulepov/ConcurrentPriorityQueue) to prioritize scanned icons.

