# FetchOptions

This can be passed to `Fetcher::Fetch` to specify details about the desired icon that the fetcher should find and download.

The default property values cause the fetcher to download the smallest icon.  The properties, especially `PerfectSize`, should be set to the desired values.


## Properties

None of the properties may be set to `null`.  Doing so will result in undefined behavior.  Either leave a property at the default or set it to the desired value.

### MinimumSize

The fetched icon will be at least this size.

```csharp
Size MinimumSize { get; set; }
```

The default value is 0x0.


### MaximumSize

The fetched icon will be at most this size.

```csharp
Size MaximumSize { get; set; }
```

The default value is reasonably big.  The exact value is undocumented (and may change).


### PerfectSize

The fetched icon will be the one closest to this size.

```csharp
Size PerfectSize { get; set; }
```

The default value is 0x0, which causes the fetcher to download the smallest icon.  It is highly recommended to set this to the desired size to optimize the fetcher.



### RequireSquare

Specifies whether to require icons to be square.

```csharp
bool RequireSquare { get; set; }
```

The default value is false, which allows rectangular favicons.
