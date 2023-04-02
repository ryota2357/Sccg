using System.Collections.Generic;
using System.IO;
using System.Text;
using Sccg.Core;

namespace Sccg.Builtin.Writers;

/// <summary>
/// Represents multiple file text content.
/// </summary>
public class MultiTextContent : IContent
{
    /// <summary>
    /// Gets the directory name.
    /// </summary>
    public string Directory { get; init; }

    /// <summary>
    /// Gets the filenames.
    /// </summary>
    public List<string> Filenames { get; init; }

    /// <summary>
    /// Gets the texts.
    /// </summary>
    public List<string> Texts { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTextContent"/> class.
    /// </summary>
    public MultiTextContent(string directory)
    {
        Directory = directory;
        Filenames = new List<string>();
        Texts = new List<string>();
    }

    /// <inheritdoc cref="MultiTextContent(string)"/>
    public MultiTextContent(string directory, string filename, string text)
    {
        Directory = directory;
        Filenames = new List<string> { filename };
        Texts = new List<string> { text };
    }

    /// <summary>
    /// Adds a <see cref="SingleTextContent"/> to this instance.
    /// </summary>
    public void Add(SingleTextContent singleTextContent)
    {
        Add(singleTextContent.Filename, singleTextContent.Text);
    }

    /// <summary>
    /// Adds a <see cref="SingleTextContent"/> to this instance.
    /// </summary>
    public void Add(string filename, string text)
    {
        Filenames.Add(filename);
        Texts.Add(text);
    }

    /// <summary>
    /// Converts to <see cref="SingleTextContent"/>s.
    /// </summary>
    public IEnumerable<SingleTextContent> ToSingleTextContents()
    {
        var result = new SingleTextContent[Filenames.Count];
        for (var i = 0; i < Filenames.Count; i++)
        {
            result[i] = new SingleTextContent(Path.Combine(Directory, Filenames[i]), Texts[i]);
        }
        return result;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder($"""
                    Class: {GetType().Name}
                    Directory: {Directory}
                    """);
        foreach (var single in ToSingleTextContents())
        {
            sb.AppendLine(single.ToString());
        }
        return sb.ToString();
    }
}