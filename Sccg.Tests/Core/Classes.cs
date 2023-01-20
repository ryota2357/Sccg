using System.Text;
using Sccg.Core;

namespace Sccg.Tests.Core;

internal class TestSourceItem : ISourceItem
{
    public string Name { get; }

    public Style? Style { get; }

    public string? Link { get; }

    public TestSourceItem(string name, Style? style = null, string? link = null)
    {
        Name = name;
        Style = style;
        Link = link;
    }
}

internal class TestDummySourceItem : ISourceItem
{
}

internal class TestContent : IContent
{
    public string Text { get; }

    public TestContent(string text)
    {
        Text = text;
    }
}

internal abstract class TestSourceBase : Source<TestSourceBase.Group, TestSourceItem>
{
    public List<TestSourceItem> Items { get; } = new();

    public override string Name => "Test";

    protected override IEnumerable<TestSourceItem> CollectItems()
    {
        return Items;
    }

    protected override void Set(Group group, Style style)
    {
        Items.Add(new TestSourceItem(group.ToString(), style));
    }

    protected override void Link(Group from, Group to)
    {
        Items.Add(new TestSourceItem(from.ToString(), link: to.ToString()));
    }

    internal enum Group
    {
        GroupA,
        GroupB,
        GroupC
    }
}

internal class TestSource : TestSourceBase
{
    public override void Custom()
    {
        Set(Group.GroupA, new Style(fg: "019ABF"));
        Set(Group.GroupB, bg: "019ABF");
        Link(Group.GroupC, Group.GroupA);
    }
}

internal class TestFormatter : Formatter<TestSourceItem, TestContent>
{
    public override string Name => "Test";

    protected override TestContent Format(IEnumerable<TestSourceItem> items)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            if (item.Style is not null)
            {
                sb.AppendLine($"{item.Name} {item.Style}");
            }
            else
            {
                sb.AppendLine($"{item.Name} {item.Link}");
            }
        }

        return new TestContent(sb.ToString());
    }
}

internal class TestWriter : Writer<TestContent>
{
    public List<string> Contents { get; } = new();

    protected override void Write(IEnumerable<TestContent> contents)
    {
        Contents.AddRange(contents.Select(content => content.Text));
    }
}