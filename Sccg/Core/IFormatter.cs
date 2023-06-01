using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a formatter for <see cref="ISource"/>.
/// </summary>
public interface IFormatter : IBuildUnit
{
    string IBuildUnit.Name => Name;
    int IBuildUnit.Priority => Priority;

    /// <summary>
    /// Gets the formatter name.
    /// </summary>
    /// <remarks>
    /// <see cref="Builder"/> cannot have more than one <see cref="IFormatter"/> with the same
    /// <see cref="IFormatter.Name"/> to avoid creating duplicate <see cref="IContent"/>.
    /// </remarks>
    public new string Name { get; }

    /// <summary>
    /// Gets the order in which the <see cref="IFormatter"/> is applied.
    /// The lower the number, the earlier the <see cref="IFormatter"/> is applied.
    /// </summary>
    public new int Priority { get; }

    /// <summary>
    /// Formats to <see cref="IContent"/> from the collection of <see cref="ISourceItem"/>.
    /// </summary>
    /// <param name="items">A collection of <see cref="ISourceItem"/> collected from all sources.</param>
    /// <param name="query">The means of accessing other formatters, etc.</param>
    /// <returns>It is passed to <see cref="IWriter"/> to write some formatted content.</returns>
    public IContent Format(IEnumerable<ISourceItem> items, BuilderQuery query);
}