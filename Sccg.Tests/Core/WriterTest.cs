using System.Text;
using Sccg.Core;

namespace Sccg.Tests.Core;

public class WriterTest
{
    [Fact]
    public void Test()
    {
        ISource source = new TestSource();
        IFormatter formatter = new TestFormatter();
        source.Custom();
        var contents = new List<IContent>()
        {
            formatter.Format(source.CollectItems())
        };

        IWriter writer = new TestWriter();
        writer.Priority.Should().Be(0);
        ((TestWriter)writer).Contents.Should().BeEmpty();
        var text = new StringBuilder()
                   .AppendLine("GroupA Style(fg:#019abf,bg:default,sp:default,default)")
                   .AppendLine("GroupB Style(fg:default,bg:#019abf,sp:default,default)")
                   .AppendLine("GroupC GroupA")
                   .ToString();

        writer.Write(contents);
        ((TestWriter)writer).Contents.Should().BeEquivalentTo(new[] { text });
        writer.Write(contents);
        ((TestWriter)writer).Contents.Should().BeEquivalentTo(new[] { text, text });
        writer.Priority.Should().Be(0);
    }
}