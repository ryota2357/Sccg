using System.Collections.Generic;
using System.IO;
using Sccg.Core;

namespace Sccg.Builtin.ContentWriters;

public class TextFileContentWriter : ContentWriter<SingleTextContent>, IMetadataUser
{
    public Metadata Metadata { get; set; } = Metadata.Empty;

    protected override void Write(IEnumerable<SingleTextContent> contents)
    {
        var outputDirectory = Metadata.Context.Get("OutputDirectory", "build");
        foreach (var content in contents)
        {
            var path = Path.Combine(outputDirectory, content.Filename);
            var directory = Path.GetDirectoryName(path);
            if (directory is not null && Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, content.Text);
        }
    }
}