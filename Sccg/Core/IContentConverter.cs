using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a converter that converts a <see cref="IContent"/>.
/// </summary>
public interface IContentConverter : IBuildUnit
{
    string IBuildUnit.Name => Name;
    int IBuildUnit.Priority => Priority;

    /// <summary>
    /// Gets the converter name.
    /// </summary>
    /// <remarks>
    /// <see cref="Builder"/> cannot have more than one <see cref="IContentConverter"/> with the same
    /// <see cref="IContentConverter.Name"/> to prevent multiple convert.
    /// </remarks>
    public new string Name { get; }

    /// <summary>
    /// Gets the order in which the <see cref="IContentConverter"/> is applied.
    /// The lower the number, the earlier the <see cref="IContentConverter"/> is applied.
    /// </summary>
    public new int Priority { get; }

    /// <summary>
    /// Converts a <see cref="IContent"/>.
    /// </summary>
    /// <param name="contents">The collection of content.</param>
    /// <param name="query">The means of accessing other converter, etc.</param>
    /// <returns>The converted content collection.</returns>
    public IEnumerable<IContent> Convert(List<IContent> contents, BuilderQuery query);
}