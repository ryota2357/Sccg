using System.Text;
using Sccg.Core;

namespace Sccg.Tests.Core;

public class FormatterTest
{
    [Fact]
    public void Test()
    {
        ISource source = new TestSource();
        source.Custom();
        var items = new List<ISourceItem> { null! };
        items.AddRange(source.CollectItems().ToList());
        items.Add(new TestDummySourceItem());

        IFormatter formatter = new TestFormatter();
        formatter.Priority.Should().Be(0);

        var result = formatter.Format(items);
        result.Should().BeOfType(typeof(TestContent));
        ((TestContent)result).Text.Should().Be(
            new StringBuilder()
                .AppendLine("GroupA Style(fg:#019abf,bg:default,sp:default,default)")
                .AppendLine("GroupB Style(fg:default,bg:#019abf,sp:default,default)")
                .AppendLine("GroupC GroupA")
                .ToString()
        );
    }
}