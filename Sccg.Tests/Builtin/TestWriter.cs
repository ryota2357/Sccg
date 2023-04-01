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
        var contentsArray = contents as IContent[] ?? contents.ToArray();
        var originalLength = contentsArray.Length;
        var singleTextContents = contentsArray.OfType<SingleTextContent>()
                                           .OrderBy(x => x.Filename)
                                           .ToArray();
        if (singleTextContents.Length != originalLength)
        {
            throw new Exception("TestWriter can only handle SingleTextContent");
        }
        foreach (var content in singleTextContents)
        {
            Output.Add(content.Text);
        }
    }
}