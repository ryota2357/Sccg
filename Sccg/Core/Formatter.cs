using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class Formatter<TSourceItem, TContent> : IFormatter
    where TSourceItem : ISourceItem
    where TContent : IContent
{
    public abstract string Name { get; }

    /// <inheritdoc cref="IFormatter.Format"/>
    protected abstract TContent Format(IEnumerable<TSourceItem> items);

    public virtual int Priority => 0;

    IContent IFormatter.Format(IEnumerable<ISourceItem> items)
    {
        return Format(items.OfType<TSourceItem>());
    }
}