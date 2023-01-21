using System.Text;
using Sccg.Core;

namespace Sccg.Tests.Core;

public class WriterTest
{
    [Fact]
    public void Test()
    {
        var query = BuilderQuery.Empty;

        ISource source = new TestSource();
        IFormatter formatter = new TestFormatter();
        source.Custom(query);
        var contents = new List<IContent>()
        {
            formatter.Format(source.CollectItems(), query)
        };

        IWriter writer = new TestWriter();
        writer.Priority.Should().Be(10);
        ((TestWriter)writer).Contents.Should().BeEmpty();
        var text = new StringBuilder()
                   .AppendLine("GroupA Style(fg:#019abf,bg:default,sp:default,default)")
                   .AppendLine("GroupB Style(fg:default,bg:#019abf,sp:default,default)")
                   .AppendLine("GroupC GroupA")
                   .ToString();

        writer.Write(contents, query);
        ((TestWriter)writer).Contents.Should().BeEquivalentTo(new[] { text });
        writer.Write(contents, query);
        ((TestWriter)writer).Contents.Should().BeEquivalentTo(new[] { text, text });
        writer.Priority.Should().Be(10);
    }
}