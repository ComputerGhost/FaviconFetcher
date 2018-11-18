# ScanResult

The possible favicons found by the scanner is returned as an enumeration of `ScanResult`s.  See Scanner.

The values of the properties are not verified or validated.  They are likely to be correct, but they may be incorrect.


## Properties



### Location

The absolute URI of the possible favicon.

```csharp
URI Location { get; }
```

The URI is not verified.  This is the URI specified by the webpage or a guessed URI.


### ExpectedSize

The expected size of the favicon.

```csharp
Size ExpectedSize { get; }
```

The size of the favicon is not verified.  This is the size specified by the webpage or a guessed size.
