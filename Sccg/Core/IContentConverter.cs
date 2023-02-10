using System.Collections.Generic;

namespace Sccg.Core;

public interface IContentConverter
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
    /// Converts a <see cref="IContent"/>.
    /// </summary>
    /// <param name="contents">The collection of content.</param>
    /// <param name="query">The means of accessing other converter, etc.</param>
    /// <returns>The converted content collection.</returns>
    public IEnumerable<IContent> Convert(List<IContent> contents, BuilderQuery query);
}