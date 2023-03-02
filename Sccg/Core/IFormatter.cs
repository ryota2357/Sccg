using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a formatter for <see cref="ISource"/>.
/// </summary>
public interface IFormatter
{
    /// <summary>
    /// Gets the formatter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the order in which the formatter is applied. The lower the number, the earlier the formatter is applied.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Formats to <see cref="IContent"/> from the collection of <see cref="ISourceItem"/>.
    /// </summary>
    /// <param name="items">A collection of <see cref="ISourceItem"/> collected from all sources.</param>
    /// <param name="query">The means of accessing other formatters, etc.</param>
    /// <returns>It is passed to <see cref="IWriter"/> to write some formatted content.</returns>
    public IContent Format(IEnumerable<ISourceItem> items, BuilderQuery query);
}