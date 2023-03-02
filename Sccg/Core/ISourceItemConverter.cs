using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a converter that converts a <see cref="ISourceItem"/>.
/// </summary>
public interface ISourceItemConverter
{
    /// <summary>
    /// Gets the converter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the order in which the converter is applied. The lower the number, the earlier the converter is applied.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Converts a <see cref="ISourceItem"/>.
    /// </summary>
    /// <param name="sourceItems">The collection of source item.</param>
    /// <param name="query">The means of accessing other converter, etc.</param>
    /// <returns>The converted source item collection.</returns>
    public IEnumerable<ISourceItem> Convert(List<ISourceItem> sourceItems, BuilderQuery query);
}