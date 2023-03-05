using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sccg.Core;

namespace Sccg;

/// <summary>
/// A query class for <see cref="Builder"/>.
/// </summary>
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
    /// <returns>Array of source. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one source.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type source and <paramref name="allowEmptyReturn"/> is false.</exception>
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
    /// <returns>Array of formatter. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one formatter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type formatter and <paramref name="allowEmptyReturn"/> is false.</exception>
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
    /// <returns>Array of writer. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one writer.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type writer and <paramref name="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetWriters<T>(bool allowEmptyReturn = false) where T : IWriter
    {
        var writer = _builder.GetWriters().TypeFilterExt<IWriter, T>();
        return allowEmptyReturn ? writer : ThrowIfEmpty("Writer", writer);
    }

    /// <summary>
    /// Gets source item converters by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the source item converter you want to get.</typeparam>
    /// <returns>Array of source item converter. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one converter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type source item converter and <paramref name="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetSourceItemConverters<T>(bool allowEmptyReturn = false)
        where T : ISourceItemConverter
    {
        var sourceItemConverter = _builder.GetSourceItemConverters().TypeFilterExt<ISourceItemConverter, T>();
        return allowEmptyReturn ? sourceItemConverter : ThrowIfEmpty("SourceItemConverter", sourceItemConverter);
    }

    /// <summary>
    /// Gets content converters by the specified type.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the content converter you want to get.</typeparam>
    /// <returns>Array of content converter. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one converter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type content converter and <paramref name="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetContentConverters<T>(bool allowEmptyReturn = false) where T : IContentConverter
    {
        var contentConverter = _builder.GetContentConverters().TypeFilterExt<IContentConverter, T>();
        return allowEmptyReturn ? contentConverter : ThrowIfEmpty("ContentConverter", contentConverter);
    }

    /// <summary>
    /// Gets the metadata of the builder.
    /// </summary>
    /// <returns>Builder metadata.</returns>
    public Metadata GetMetadata()
    {
        return _builder.Metadata;
    }

    /// <summary>
    /// Gets the source items of the builder.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the source item you want to get.</typeparam>
    /// <returns>Array of source item. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one converter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type source item and <paramref name="allowEmptyReturn"/> is false.</exception>
    public ReadOnlyCollection<T> GetSourceItems<T>(bool allowEmptyReturn = false) where T : ISourceItem
    {
        var sourceItems = _builder.GetSourceItems().TypeFilterExt<ISourceItem, T>();
        return allowEmptyReturn ? sourceItems : ThrowIfEmpty("SourceItem", sourceItems);
    }

    /// <summary>
    /// Gets the contents of the builder.
    /// </summary>
    /// <param name="allowEmptyReturn">If true, the return value array is allowed to be empty.</param>
    /// <typeparam name="T">A type of the content you want to get.</typeparam>
    /// <returns>Array of content. If <paramref name="allowEmptyReturn"/> is false, the array will contain at least one converter.</returns>
    /// <exception cref="InvalidOperationException">Not found the specified type content and <paramref name="allowEmptyReturn"/> is false.</exception>
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

    /// <summary>
    /// Registers the specified source item converter to builder.
    /// </summary>
    /// <param name="sourceItemConverter"></param>
    public void RegisterSourceItemConverter(ISourceItemConverter sourceItemConverter)
    {
        _builder.Use(sourceItemConverter);
    }

    /// <summary>
    /// Registers the specified content converter to builder.
    /// </summary>
    /// <param name="contentConverter"></param>
    public void RegisterContentConverter(IContentConverter contentConverter)
    {
        _builder.Use(contentConverter);
    }

    private static ReadOnlyCollection<T> ThrowIfEmpty<T>(string type, in ReadOnlyCollection<T> value)
    {
        if (value.Count == 0)
        {
            throw new InvalidOperationException($"No {type} of type `{typeof(T).FullName}` was found.");
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