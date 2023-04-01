using System.Collections.Generic;
using System.IO;
using System.Text;
using Sccg.Core;

namespace Sccg.Builtin.Writers;

public class MultiTextContent : IContent
{
    public string Directory { get; init; }
    public List<string> Filenames { get; init; }
    public List<string> Texts { get; init; }

    public MultiTextContent(string directory)
    {
        Directory = directory;
        Filenames = new List<string>();
        Texts = new List<string>();
    }

    public MultiTextContent(string directory, string filename, string text)
    {
        Directory = directory;
        Filenames = new List<string> { filename };
        Texts = new List<string> { text };
    }

    public void Add(SingleTextContent singleTextContent)
    {
        Add(singleTextContent.Filename, singleTextContent.Text);
    }

    public void Add(string filename, string text)
    {
        Filenames.Add(filename);
        Texts.Add(text);
    }

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