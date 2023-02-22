using System.Collections.Generic;
using System.Linq;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Writers;

public class SingleTextContent : IContent
{
    public string Filename { get; init; }
    public string Text { get; init; }

    public SingleTextContent(string filename, string text)
    {
        Filename = filename;
        Text = text;
    }

    public SingleTextContent(string filename, params string?[] texts)
        : this(filename, string.Join('\n', texts.WhereNotNull().DefaultIfEmpty("")))
    {
    }

    public SingleTextContent(string filename, IEnumerable<string?> texts)
        : this(filename, string.Join('\n', texts.WhereNotNull().DefaultIfEmpty("")))
    {
    }

    public override string ToString()
    {
        return $"""
                Class: {GetType().Name}
                Filename: {Filename}
                Text: {Text}
                """;
    }
}