using System;
using System.Collections;
using System.Collections.Generic;

namespace Sccg;

/// <summary>
/// Metadata of theme and Builder settings.
/// </summary>
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
    /// <summary>
    /// My name (ryota2357)
    /// </summary>
    public const string __SccgDeveloper = "ryota2357";

    /// <summary>
    /// Sccg version.
    /// </summary>
    public const string __SccgVersion = "0.3.1";

    /// <summary>
    /// Empty metadata. All properties are null or default object.
    /// </summary>
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

    /// <summary>
    /// Default metadata.
    /// </summary>
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

/// <summary>
/// Type of theme.
/// </summary>
public enum ThemeType
{
    /// <summary>
    /// Light theme.
    /// </summary>
    Light,

    /// <summary>
    /// Dark theme.
    /// </summary>
    Dark
}

/// <summary>
/// Context of <see cref="Metadata"/>.
/// </summary>
public sealed class MetadataContext : IEnumerable<KeyValuePair<string, object?>>
{
    private readonly Dictionary<string, object?> _data = new();

    /// <summary>
    /// Gets the number of key/value pairs contained in the <see cref="MetadataContext"/>.
    /// </summary>
    public int Count => _data.Count;

    /// <summary>
    /// Gets a collection containing the keys in the <see cref="MetadataContext"/>.
    /// </summary>
    public ICollection<string> Keys => _data.Keys;

    /// <summary>
    /// Gets a collection containing the values in the <see cref="MetadataContext"/>.
    /// </summary>
    public ICollection<object?> Values => _data.Values;

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    public void Add(string key, object? value)
    {
        _data.Add(key, value);
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <exception cref="ArgumentException">The value for <paramref name="key"/> is not of <typeparamref name="T"/></exception>
    public T Get<T>(string key, T defaultValue)
    {
        if (!_data.TryGetValue(key, out var value)) return defaultValue;
        if (value is T t)
        {
            return t;
        }

        throw new ArgumentException($"The value for key '{key}' is not of type '{typeof(T).Name}'.");
    }

    /// <summary>
    /// Determines whether the <see cref="MetadataContext"/> contains the specified key
    /// </summary>
    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}