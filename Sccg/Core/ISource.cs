using System.Collections.Generic;

namespace Sccg.Core;

public interface ISource
{
    /// <summary>
    /// Gets the source name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the order in which the source is applied. The lower the number, the earlier the source is applied.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Collects source items from the source.
    /// </summary>
    /// <returns>It is passed to <see cref="IFormatter"/> to format.</returns>
    public IEnumerable<ISourceItem> CollectItems();

    /// <summary>
    /// Starts the source customization.
    /// </summary>
    /// <param name="query">The means of accessing other sources, etc.</param>
    public void Custom(BuilderQuery query);
}