using Sccg.Builtin.Writers;
using Sccg.Core;

namespace Sccg.Tests.Builtin;

public class TestWriter : IWriter
{
    public List<string> Output { get; } = new();

    public string Name => "Test";

    public int Priority => 0;

    public void Write(IEnumerable<IContent> contents, BuilderQuery query)
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