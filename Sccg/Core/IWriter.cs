using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// Represents a writer that writes the content to some form.
/// </summary>
public interface IWriter
{
    /// <summary>
    /// Gets the writer name.
    /// </summary>
    /// <remarks>
    /// <see cref="Builder"/> cannot have more than one <see cref="IWriter"/> with the same <see cref="Name"/> to avoid
    /// creating duplicate output.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Gets the order in which the writer is applied. The lower the number, the earlier the writer is applied.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Writes the specified content.
    /// </summary>
    /// <param name="contents">A collection of <see cref="IContent"/> collected from all formatters.</param>
    /// <param name="query">The means of accessing other writers, etc.</param>
    public void Write(IEnumerable<IContent> contents, BuilderQuery query);
}