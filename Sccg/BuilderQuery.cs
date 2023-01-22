using System;
using System.Collections.Generic;
using System.Linq;
using Sccg.Core;

namespace Sccg;

public sealed class BuilderQuery
{
    private readonly List<IFormatter> _formatters = new();
    private readonly List<ISource> _sources = new();
    private readonly List<IWriter> _writers = new();
    private Metadata _metadata = Metadata.Empty;

    private List<ISourceItem>? _sourceItemsCache = null;
    private List<IContent>? _contentsCache = null;

    internal static BuilderQuery Empty => new();

    internal BuilderQuery()
    {
    }

    /// <summary>
    /// Gets sources by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the source you want to get.</typeparam>
    /// <returns>Array of source. If <see cref="allowEmptyReturn"/> is false, the array will contain at least one source.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type source and <see cref="allowEmptyReturn"/> is false.</exception>
    public T[] GetSources<T>(bool allowEmptyReturn = false) where T : ISource
    {
        var source = _sources.OfType<T>().ToArray();
        return allowEmptyReturn ? source : ThrowIfEmpty("Source", source);
    }

    /// <summary>
    /// Gets formatters by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the formatter you want to get.</typeparam>
    /// <returns>Array of formatter. If <see cref="allowEmptyReturn"/> is false, the array will contain at least one formatter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type formatter and <see cref="allowEmptyReturn"/> is false.</exception>
    public T[] GetFormatters<T>(bool allowEmptyReturn = false) where T : IFormatter
    {
        var formatters = _formatters.OfType<T>().ToArray();
        return allowEmptyReturn ? formatters : ThrowIfEmpty("Formatter", formatters);
    }

    /// <summary>
    /// Gets writers by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the writer you want to get.</typeparam>
    /// <returns>Array of writer. If <see cref="allowEmptyReturn"/> is false, the array will contain at least one writer.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type writer and <see cref="allowEmptyReturn"/> is false.</exception>
    public T[] GetWriters<T>(bool allowEmptyReturn = false) where T : IWriter
    {
        var writer = _writers.OfType<T>().ToArray();
        return allowEmptyReturn ? writer : ThrowIfEmpty("Writer", writer);
    }

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    /// <returns>Builder metadata.</returns>
    public Metadata GetMetadata()
    {
        return _metadata;
    }

    // TODO: new impl
    public T[] GetSourceItems<T>(bool allowEmptyReturn = false) where T : ISourceItem
    {
        CacheSourceItems();
        var sourceItems = _sourceItemsCache!.OfType<T>().ToArray();
        return allowEmptyReturn ? sourceItems : ThrowIfEmpty("SourceItem", sourceItems);
    }

    // TODO: new impl
    public T[] GetContents<T>(bool allowEmptyReturn = false) where T : IContent
    {
        CacheContents();
        var contents = _contentsCache!.OfType<T>().ToArray();
        return allowEmptyReturn ? contents : ThrowIfEmpty("Content", contents);
    }

    /// <summary>
    /// Registers the specified source to builder.
    /// </summary>
    /// <param name="source">A instance of source.</param>
    public void RegisterSource(ISource source)
    {
        _sources.Add(source);
        _sources.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _sourceItemsCache = null;
    }

    /// <summary>
    /// Registers the specified formatter to builder.
    /// </summary>
    /// <param name="formatter">A instance of formatter.</param>
    public void RegisterFormatter(IFormatter formatter)
    {
        _formatters.Add(formatter);
        _formatters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _contentsCache = null;
    }

    /// <summary>
    /// Registers the specified writer to builder.
    /// </summary>
    /// <param name="writer">A instance of writer.</param>
    public void RegisterWriter(IWriter writer)
    {
        _writers.Add(writer);
        _writers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }

    internal void RegisterMetadata(Metadata metadata)
    {
        _metadata = metadata;
    }

    private void CacheSourceItems()
    {
        if (_sourceItemsCache is not null)
        {
            return;
        }

        _sourceItemsCache = new List<ISourceItem>();
        foreach (var source in _sources)
        {
            IEnumerable<ISourceItem> items;
            try
            {
                source.Custom(this);
                items = source.CollectItems();
            }
            catch (Exception e)
            {
                throw new Exception($"Source {source.Name} failed.", e);
            }
            _sourceItemsCache.AddRange(items);
        }
    }

    private void CacheContents()
    {
        if (_contentsCache is not null)
        {
            return;
        }

        CacheSourceItems();

        _contentsCache = new List<IContent>();
        foreach (var formatter in _formatters)
        {
            IContent content;
            try
            {
                // NOTE: _sourceItemsCache is not null here because CacheSourceItems() is called before
                content = formatter.Format(_sourceItemsCache!, this);
            }
            catch (Exception e)
            {
                throw new Exception($"Formatter {formatter.Name} failed.", e);
            }
            _contentsCache.Add(content);
        }
    }

    private static T[] ThrowIfEmpty<T>(string type, in T[] value)
    {
        if (value.Length == 0)
        {
            throw new InvalidOperationException($"No ${type} of type {typeof(T).Name} was found.");
        }
        return value;
    }
}