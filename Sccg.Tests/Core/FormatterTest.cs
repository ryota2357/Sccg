using System.Text;
using Sccg.Core;

namespace Sccg.Tests.Core;

public class FormatterTest
{
    [Fact]
    public void Test()
    {
        IFormatter formatter = new TestFormatter();
        var query = new BuilderQuery(new Builder());
        var items = new ISourceItem[]
        {
            new TestSourceItem("A", "propA"),
            new TestSourceItem("B", "propB"),
            new TestDummySourceItem(),
            new TestSourceItem("C", "propC"),
            new TestDummySourceItem(),
        };

        formatter.Priority.Should().Be(10);
        formatter.Name.Should().Be("Test");

        var result = formatter.Format(items, query);
        result.Should().BeOfType(typeof(TestContent));
        ((TestContent)result).Text.Should().Be("""
            A:propA
            B:propB
            C:propC
            """
        );

        formatter.Priority.Should().Be(10);
        formatter.Name.Should().Be("Test");
    }

    private class TestSourceItem : ISourceItem
    {
        public string Name { get; }
        public string Property { get; }
        public TestSourceItem(string name, string property)
        {
            Name = name;
            Property = property;
        }
    }

    private class TestDummySourceItem : ISourceItem
    {
    }

    private class TestContent : IContent
    {
        public string Text { get; }
        public TestContent(string text)
        {
            Text = text;
        }
    }

    private class TestFormatter : Formatter<TestSourceItem, TestContent>
    {
        public override string Name => "Test";

        protected override TestContent Format(IEnumerable<TestSourceItem> items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.AppendLine($"{item.Name}:{item.Property}");
            }
            return new TestContent(sb.ToString().Trim());
        }
    }
}