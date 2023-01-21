using System;
using System.Collections.Generic;
using System.Linq;
using Sccg.Core;

namespace Sccg;

public class BuilderQuery
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
    /// TODO: doc
    /// </summary>
    /// <param name="allowEmptyReturn"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] GetFormatters<T>(bool allowEmptyReturn = false) where T : IFormatter
    {
        var formatters = _formatters.OfType<T>().ToArray() ?? Array.Empty<T>();
        return allowEmptyReturn ? formatters : ThrowIfEmpty("Formatter", formatters);
    }

    /// <inheritdoc cref="GetFormatters{T}(bool)"/>
    /// <param name="predicate"></param>
    /// <param name="allowEmptyReturn"></param>
    public T[] GetFormatters<T>(Func<T, bool> predicate, bool allowEmptyReturn = false) where T : IFormatter
    {
        var formatters = _formatters.OfType<T>().Where(predicate).ToArray();
        return allowEmptyReturn ? formatters : ThrowIfEmpty("Formatter", formatters);
    }

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="allowEmptyReturn"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T[] GetSources<T>(bool allowEmptyReturn = false) where T : ISource
    {
        var source = _sources.OfType<T>().ToArray();
        return allowEmptyReturn ? source : ThrowIfEmpty("Source", source);
    }

    /// <inheritdoc cref="GetSources{T}(bool)"/>
    /// <param name="predicate"></param>
    /// <param name="allowEmptyReturn"></param>
    public T[] GetSources<T>(Func<T, bool> predicate, bool allowEmptyReturn = false) where T : ISource
    {
        var source = _sources.OfType<T>().Where(predicate).ToArray();
        return allowEmptyReturn ? source : ThrowIfEmpty("Source", source);
    }

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="allowEmptyReturn"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] GetSourceItems<T>(bool allowEmptyReturn = false) where T : ISourceItem
    {
        CacheSourceItems();
        var sourceItems = _sourceItemsCache!.OfType<T>().ToArray();
        return allowEmptyReturn ? sourceItems : ThrowIfEmpty("SourceItem", sourceItems);
    }

    /// <inheritdoc cref="GetSourceItems{T}(bool)"/>
    /// <param name="predicate"></param>
    /// <param name="allowEmptyReturn"></param>
    public T[] GetSourceItems<T>(Func<T, bool> predicate, bool allowEmptyReturn = false) where T : ISourceItem
    {
        CacheSourceItems();
        var sourceItems = _sourceItemsCache!.OfType<T>().Where(predicate).ToArray();
        return allowEmptyReturn ? sourceItems : ThrowIfEmpty("SourceItem", sourceItems);
    }

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="allowEmptyReturn"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] GetContents<T>(bool allowEmptyReturn = false) where T : IContent
    {
        CacheContents();
        var contents = _contentsCache!.OfType<T>().ToArray();
        return allowEmptyReturn ? contents : ThrowIfEmpty("Content", contents);
    }

    /// <inheritdoc cref="GetContents{T}(bool)"/>
    /// <param name="predicate"></param>
    /// <param name="allowEmptyReturn"></param>
    public T[] GetContents<T>(Func<T, bool> predicate, bool allowEmptyReturn = false) where T : IContent
    {
        CacheContents();
        var contents = _contentsCache!.OfType<T>().Where(predicate).ToArray();
        return allowEmptyReturn ? contents : ThrowIfEmpty("Content", contents);
    }

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] GetWriters<T>() where T : IWriter
    {
        var writer = _writers.OfType<T>().ToArray();
        ThrowIfEmpty("Writer", writer);
        return writer;
    }

    /// <inheritdoc cref="GetWriters{T}()"/>
    /// <param name="predicate"></param>
    public T[] GetWriters<T>(Func<T, bool> predicate) where T : IWriter
    {
        var writer = _writers.OfType<T>().Where(predicate).ToArray();
        ThrowIfEmpty("Writer", writer);
        return writer;
    }

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <returns></returns>
    public Metadata GetMetadata()
    {
        return _metadata;
    }

    public void RegisterSource(ISource source)
    {
        _sources.Add(source);
        _sources.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _sourceItemsCache = null;
    }

    public void RegisterFormatter(IFormatter formatter)
    {
        _formatters.Add(formatter);
        _formatters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _contentsCache = null;
    }

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