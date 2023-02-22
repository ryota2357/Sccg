using System;
using System.Collections;
using System.Collections.Generic;

namespace Sccg;

public sealed record Metadata(
    string? ThemeName,
    ThemeType? ThemeType,
    string? Version,
    string? Author,
    string? Maintainer,
    string? Homepage,
    string? Repository,
    string? Description,
    string? License,
    DateTime? LastUpdated,
    Func<Metadata, string?[]?> Header,
    Func<Metadata, string?[]?> Footer,
    MetadataContext Context)
{
    public const string __SccgDeveloper = "ryota2357";
    public const string __SccgVersion = "0.0.1";

    public static Metadata Empty => new(
        ThemeName: null,
        ThemeType: null,
        Version: null,
        Author: null,
        Maintainer: null,
        Homepage: null,
        Repository: null,
        Description: null,
        License: null,
        LastUpdated: null,
        Header: _ => Array.Empty<string>(),
        Footer: _ => Array.Empty<string>(),
        Context: new MetadataContext()
    );

    public static Metadata Default => new(
        ThemeName: null,
        ThemeType: null,
        Version: "1.0.0",
        Author: null,
        Maintainer: null,
        Homepage: null,
        Repository: null,
        Description: null,
        License: null,
        LastUpdated: DateTime.Now,
        Header: _ => null,
        Footer: _ => new[] { $"Built with Sccg {__SccgVersion}" },
        Context: new MetadataContext()
    );
}

public enum ThemeType
{
    Light,
    Dark
}

public sealed class MetadataContext : IEnumerable<KeyValuePair<string, object?>>
{
    private readonly Dictionary<string, object?> _data = new();

    public int Count => _data.Count;

    public ICollection<string> Keys => _data.Keys;

    public ICollection<object?> Values => _data.Values;

    public void Add(string key, object? value)
    {
        _data.Add(key, value);
    }

    public T Get<T>(string key, T defaultValue)
    {
        if (!_data.TryGetValue(key, out var value)) return defaultValue;
        if (value is T t)
        {
            return t;
        }

        throw new ArgumentException($"The value for key '{key}' is not of type '{typeof(T).Name}'.");
    }

    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}