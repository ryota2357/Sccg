using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sccg.Core;

namespace Sccg;

public sealed class BuilderQuery
{
    private readonly Builder _builder;

    internal BuilderQuery(Builder builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// Gets sources by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the source you want to get.</typeparam>
    /// <returns>Array of source. If <see cref="allowEmptyReturn"/> is false, the array will contain at least one source.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type source and <see cref="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetSources<T>(bool allowEmptyReturn = false) where T : ISource
    {
        var source = _builder.GetSources().TypeFilterExt<ISource, T>();
        return allowEmptyReturn ? source : ThrowIfEmpty("Source", source);
    }

    /// <summary>
    /// Gets formatters by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the formatter you want to get.</typeparam>
    /// <returns>Array of formatter. If <see cref="allowEmptyReturn"/> is false, the array will contain at least one formatter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type formatter and <see cref="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetFormatters<T>(bool allowEmptyReturn = false) where T : IFormatter
    {
        var formatters = _builder.GetFormatters().TypeFilterExt<IFormatter, T>();
        return allowEmptyReturn ? formatters : ThrowIfEmpty("Formatter", formatters);
    }

    /// <summary>
    /// Gets writers by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the writer you want to get.</typeparam>
    /// <returns>Array of writer. If <see cref="allowEmptyReturn"/> is false, the array will contain at least one writer.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type writer and <see cref="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetWriters<T>(bool allowEmptyReturn = false) where T : IWriter
    {
        var writer = _builder.GetWriters().TypeFilterExt<IWriter, T>();
        return allowEmptyReturn ? writer : ThrowIfEmpty("Writer", writer);
    }

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    /// <returns>Builder metadata.</returns>
    public Metadata GetMetadata()
    {
        return _builder.Metadata;
    }

    // TODO: doc
    public ReadOnlyCollection<T> GetSourceItems<T>(bool allowEmptyReturn = false) where T : ISourceItem
    {
        var sourceItems = _builder.GetSourceItems().TypeFilterExt<ISourceItem, T>();
        return allowEmptyReturn ? sourceItems : ThrowIfEmpty("SourceItem", sourceItems);
    }

    // TODO: doc
    public ReadOnlyCollection<T> GetContents<T>(bool allowEmptyReturn = false) where T : IContent
    {
        var contents = _builder.GetContents().TypeFilterExt<IContent, T>();
        return allowEmptyReturn ? contents : ThrowIfEmpty("Content", contents);
    }

    /// <summary>
    /// Registers the specified source to builder.
    /// </summary>
    /// <param name="source">A instance of source.</param>
    public void RegisterSource(ISource source)
    {
         _builder.Use(source);
    }

    /// <summary>
    /// Registers the specified formatter to builder.
    /// </summary>
    /// <param name="formatter">A instance of formatter.</param>
    public void RegisterFormatter(IFormatter formatter)
    {
        _builder.Use(formatter);
    }

    /// <summary>
    /// Registers the specified writer to builder.
    /// </summary>
    /// <param name="writer">A instance of writer.</param>
    public void RegisterWriter(IWriter writer)
    {
        _builder.Use(writer);
    }

    private static ReadOnlyCollection<T> ThrowIfEmpty<T>(string type, in ReadOnlyCollection<T> value)
    {
        if (value.Count == 0)
        {
            throw new InvalidOperationException($"No {type} of type {typeof(T).Name} was found.");
        }
        return value;
    }
}

file static class Ext
{
    public static ReadOnlyCollection<TResult> TypeFilterExt<TSource, TResult>(this IEnumerable<TSource> source)
    {
        return source.OfType<TResult>().ToList().AsReadOnly();
    }
}