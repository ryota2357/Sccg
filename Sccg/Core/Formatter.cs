using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class Formatter<TSourceItem, TContent> : IFormatter
    where TSourceItem : ISourceItem
    where TContent : IContent
{
    public abstract string Name { get; }

    public virtual int Priority => 10;

    /// <inheritdoc cref="IFormatter.Format"/>
    protected virtual TContent Format(IEnumerable<TSourceItem> items)
    {
        throw new NotImplementedException("You must override Format method.");
    }

    protected virtual TContent Format(IEnumerable<TSourceItem> items, BuilderQuery query)
    {
        return Format(items);
    }

    private IContent? _format = null;
    IContent IFormatter.Format(IEnumerable<ISourceItem> items, BuilderQuery query)
    {
        return _format ??= Format(items.OfType<TSourceItem>(), query);
    }
}