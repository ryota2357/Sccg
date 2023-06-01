using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a source that provides  source items.
/// </summary>
public interface ISource : IBuildUnit
{
    string IBuildUnit.Name => Name;
    int IBuildUnit.Priority => Priority;

    /// <summary>
    /// Gets the source name.
    /// </summary>
    /// <remarks>
    /// <see cref="Builder"/> cannot have more than one <see cref="ISource"/> with the same <see cref="ISource.Name"/>to
    /// avoid creating duplicate <see cref="ISourceItem"/>.
    /// </remarks>
    public new string Name { get; }

    /// <summary>
    /// Gets the order in which the <see cref="ISource"/> is applied.
    /// The lower the number, the earlier the <see cref="ISource"/> is applied.
    /// </summary>
    public new int Priority { get; }

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