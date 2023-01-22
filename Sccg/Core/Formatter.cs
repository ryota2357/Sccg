using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class Formatter<TSourceItem, TContent> : IFormatter
    where TSourceItem : ISourceItem
    where TContent : IContent
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public virtual int Priority => 10;

    // TODO: doc
    protected virtual TContent Format(IEnumerable<TSourceItem> items)
    {
        throw new NotImplementedException("You must override Format method.");
    }

    // TODO: doc
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