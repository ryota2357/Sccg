using System.Collections.Generic;
using System.IO;
using Sccg.Core;

namespace Sccg.Builtin.Writers;

/// <summary>
/// A writer that writes text files (<see cref="SingleTextContent"/>).
/// </summary>
public class TextFileWriter : Writer<SingleTextContent>
{
    public override string Name => "TextFile";

    /// <inheritdoc />
    protected override void Write(IEnumerable<SingleTextContent> contents, BuilderQuery query)
    {
        var outputDirectory = query.GetMetadata().Context.Get("OutputDirectory", "build");
        foreach (var content in contents)
        {
            var path = Path.Combine(outputDirectory, content.Filename);
            var directory = Path.GetDirectoryName(path);
            if (directory is not null && Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, content.Text);
        }
    }
}