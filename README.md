# Favicon Fetcher

FaviconFetcher contains tools for scanning a webpage for possible favicons (Scanner) and downloading the perfect one (Fetcher).

This library is perfect for non-browsers that need to download a favicon for a webpage.


## Features

Favicon fetcher was built to include all needed functionality with ease of use and efficiency in mind.

 * Resources are prioritized and downloaded on demand.
 * Cache shared with other applications (off by default).
 * Supports manifest.json and browserconfig.xml, if linked from the HTML.
 * Easy to use.


## Installation

### Method 1 (Easy)

   1. Use Nuget Package Manager to install "FaviconFetcher".

### Method 2 (Hard)

   1. Download the source code.
   2. Compile it.  If compiling as "Release", ignore the errors in the FaviconFetcher.Tests project.
   3. Copy the library file to where you want it.
   4. Add a reference to it from your own project.


## How to Use

Common uses are listed below.  The full documentation is at <https://github.com/ComputerGhost/FaviconFetcher/tree/master/doc>.  In places where the code differs from the documentation, the documentation is correct.


### Fetcher - Find the Best Match

The fetcher will scan a webpage for favicons and download the one that best matches the given constraints.

```csharp
var fetcher = new Fetcher();
var image = await fetcher.FetchClosest(uri, new Size(16, 16));
// Don't forget to dispose of the image when no longer needed.
```

Other methods include `FetchExact` and `Fetch`.  See the documentation for the full list and how to use them.


### Scanner - Get a List of Favicons

To get a list of possible favicons without downloading any, use the scanner.

```csharp
var scanner = new Scanner();
foreach (var result in await scanner.Scan(uri))
{
    var expectedSize = result.ExpectedSize;
    var absoluteUri = result.Location;
    // do something with it.
}
```

Keep in mind that an icon may not exist, may be invalid, may be a different size than the one reported, or may be a duplicate.  Unlike the fetcher, the scanner does not verify that its results are correct.


### HttpSource - Caching

Caching is supported by both the fetcher and the scanner, but it is off by default.  To enable caching, pass a configured `HttpSource` to the constructor.

```csharp
var source = new HttpSource() {
    CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable)
};
var fetcher = new Fetcher(source);
var image = await fetcher.FetchClosest(uri, new Size(16, 16));
// Don't forget to dispose of the image when no longer needed.
```


## Bug Reports

Please use the Issues tab on our GitHub page to file bug reports. Or, you may contact me directly if you know my contact information.


## Credits

 * [dshulepov](https://github.com/dshulepov) - Our priority queue code comes from them.  The original uri was <https://github.com/dshulepov/ConcurrentPriorityQueue>.
 * [kurema](https://github.com/kurema) - Updated dependencies.  Pulled in priority queue from other repo to remove requirement for old framework.
 * [florian-berger](https://github.com/florian-berger) - Added proxy functionality.
