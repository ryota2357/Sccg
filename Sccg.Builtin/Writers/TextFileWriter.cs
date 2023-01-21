using System.Collections.Generic;
using System.IO;
using Sccg.Core;

namespace Sccg.Builtin.Writers;

public class TextFileWriter : Writer<SingleTextContent>
{
    protected override void Write(IEnumerable<SingleTextContent> contents, BuilderQuery query)
    {
        var outputDirectory = query.GetMetadata().Context.Get("OutputDirectory", "build");
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