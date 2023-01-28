using Sccg.Core;

namespace Sccg.Tests.Core;

public class WriterTest
{
    [Fact]
    public void Test()
    {
        IWriter writer = new TestWriter();
        var query = new BuilderQuery(new Builder());
        var contents = new IContent[]
        {
            new TestDummyContent(),
            new TestContent("hoge"),
            new TestContent("fuga"),
            new TestDummyContent(),
            new TestContent("piyo"),
        };
        var text = new[]
        {
            "hoge",
            "fuga",
            "piyo",
        };

        writer.Priority.Should().Be(10);
        writer.Name.Should().Be("Test");
        ((TestWriter)writer).Contents.Should().BeEmpty();

        writer.Write(contents, query);
        ((TestWriter)writer).Contents.Should().BeEquivalentTo(text);
        writer.Write(contents, query);
        ((TestWriter)writer).Contents.Should().BeEquivalentTo(text.Concat(text));

        writer.Priority.Should().Be(10);
        writer.Name.Should().Be("Test");
    }

    private class TestContent : IContent
    {
        public string Text { get; }
        public TestContent(string text)
        {
            Text = text;
        }
    }

    private class TestDummyContent : IContent
    {
    }

    private class TestWriter : Writer<TestContent>
    {
        public override string Name => "Test";

        public List<string> Contents { get; } = new();

        protected override void Write(IEnumerable<TestContent> contents)
        {
            Contents.AddRange(contents.Select(content => content.Text));
        }
    }
}