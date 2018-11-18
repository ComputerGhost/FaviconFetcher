# ISource

A source may be specified for use by the scanner and fetcher to download resources.  This is the interface that that source must implement.

For an example, see `MockSource` in the unit test code at <https://github.com/ComputerGhost/FaviconFetcher/blob/master/FaviconFetcher.Tests/Utility/MockSource.cs>.


## Methods


### DownloadText

Downloads a text resource to be read.

```csharp
StreamReader DownloadText(Uri uri)
```

If the resource cannot be retrieved, then `null` should be returned to skip it.  If further resources should not be requested, then an exception should be thrown.


### DownloadImages

Downloads a all images at a URI.

```csharp
IEnumerable<Image> DownloadImages(Uri uri)
```

Some files may contain multiple images.  All images within the file should be returned.

If the resource cannot be retrieved, then an empty set (not `null`) should be returned to skip it.  If further resources should not be requested, then an exception should be thrown.
