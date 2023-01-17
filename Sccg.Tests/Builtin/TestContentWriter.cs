using Sccg.Builtin.ContentWriters;
using Sccg.Core;

namespace Sccg.Tests.Builtin;

public class TestContentWriter : IContentWriter
{
    public List<string> Output { get; } = new();

    public int Priority => 0;

    public void Write(IEnumerable<IContent> contents)
    {
        foreach (var content in contents)
        {
            if (content is SingleTextContent singleTextContent)
            {
                Output.Add(singleTextContent.Text);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}