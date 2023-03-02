using System.Collections.Generic;
using System.Linq;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Writers;

/// <summary>
/// Represents a single file text content.
/// </summary>
public class SingleTextContent : IContent
{
    /// <summary>
    /// Gets the filename.
    /// </summary>
    public string Filename { get; init; }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleTextContent"/> class.
    /// </summary>
    public SingleTextContent(string filename, string text)
    {
        Filename = filename;
        Text = text;
    }

    /// <inheritdoc cref="SingleTextContent(string,string)"/>
    public SingleTextContent(string filename, params string?[] texts)
        : this(filename, string.Join('\n', texts.WhereNotNull().DefaultIfEmpty("")))
    {
    }

    /// <inheritdoc cref="SingleTextContent(string,string)"/>
    public SingleTextContent(string filename, IEnumerable<string?> texts)
        : this(filename, string.Join('\n', texts.WhereNotNull().DefaultIfEmpty("")))
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"""
                Class: {GetType().Name}
                Filename: {Filename}
                Text: {Text}
                """;
    }
}