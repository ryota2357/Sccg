using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

/// <summary>
/// The base class of <see cref="IFormatter"/>.
/// </summary>
public abstract class Formatter<TSourceItem, TContent> : IFormatter
    where TSourceItem : ISourceItem
    where TContent : IContent
{
    /// <inheritdoc cref="IFormatter.Name"/>
    public abstract string Name { get; }

    /// <inheritdoc cref="IFormatter.Priority"/>
    public virtual int Priority => 10;

    /// <summary>
    /// Formats to <typeparamref name="TContent"/> from the collection of <typeparamref name="TSourceItem"/>.
    /// </summary>
    /// <param name="items">A collection of <typeparamref name="TSourceItem"/> collected from all sources.</param>
    /// <returns>It is passed to <see cref="IWriter"/> to write some formatted content.</returns>
    protected virtual TContent Format(IEnumerable<TSourceItem> items)
    {
        throw new NotImplementedException("You must override Format method.");
    }

    /// <summary>
    /// Formats to <typeparamref name="TContent"/> from the collection of <typeparamref name="TSourceItem"/>.
    /// </summary>
    /// <param name="items">A collection of <typeparamref name="TSourceItem"/> collected from all sources.</param>
    /// <param name="query">The means of accessing other formatters, etc.</param>
    /// <returns>It is passed to <see cref="IWriter"/> to write some formatted content.</returns>
    protected virtual TContent Format(IEnumerable<TSourceItem> items, BuilderQuery query)
    {
        return Format(items);
    }

    // This cache is necessary to respect Priority.
    private IContent? _format = null;

    /// <inheritdoc />
    IContent IFormatter.Format(IEnumerable<ISourceItem> items, BuilderQuery query)
    {
        return _format ??= Format(items.OfType<TSourceItem>(), query);
    }
}