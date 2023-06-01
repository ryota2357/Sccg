using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a converter that converts a <see cref="ISourceItem"/>.
/// </summary>
public interface ISourceItemConverter : IBuildUnit
{
    string IBuildUnit.Name => Name;
    int IBuildUnit.Priority => Priority;

    /// <summary>
    /// Gets the converter name.
    /// </summary>
    /// <remarks>
    /// <see cref="Builder"/> cannot have more than one <see cref="ISourceItemConverter"/> with the same
    /// <see cref="ISourceItemConverter.Name"/> to prevent multiple convert.
    /// </remarks>
    public new string Name { get; }

    /// <summary>
    /// Gets the order in which the <see cref="ISourceItemConverter"/> is applied.
    /// The lower the number, the earlier the <see cref="ISourceItemConverter"/> is applied.
    /// </summary>
    public new int Priority { get; }

    /// <summary>
    /// Converts a <see cref="ISourceItem"/>.
    /// </summary>
    /// <param name="sourceItems">The collection of source item.</param>
    /// <param name="query">The means of accessing other converter, etc.</param>
    /// <returns>The converted source item collection.</returns>
    public IEnumerable<ISourceItem> Convert(List<ISourceItem> sourceItems, BuilderQuery query);
}