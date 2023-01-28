using Sccg.Core;

namespace Sccg.Tests.Core;

public class SourceTest
{
    [Fact]
    public void Test()
    {
        ISource source = new TestSource();
        var query = new BuilderQuery(new Builder());

        source.Priority.Should().Be(10);
        source.Name.Should().Be("Test");
        ((TestSource)source).Items.Should().BeEmpty();

        source.Custom(query);
        source.Priority.Should().Be(10);
        source.Name.Should().Be("Test");
        ((TestSource)source).Items.Should().BeEquivalentTo(new[]
        {
            new TestSourceItem("GroupA", style: new Style(fg: "019abf")),
            new TestSourceItem("GroupB", style: new Style(bg: "019abf")),
            new TestSourceItem("GroupC", link: "GroupA"),
        });
    }

    [Fact]
    public void Set()
    {
        var source = new SetTestSource();
        var query = new BuilderQuery(new Builder());

        source.Styles.Should().BeEmpty();
        ((ISource)source).Custom(query);
        source.Styles.Should().BeEquivalentTo(new[]
        {
            new Style(fg: "1a09bf"),
            new Style(bg: "1a09bf"),
            new Style(sp: "1a09bf"),
            new Style(none: true),
            new Style(bold: true),
            new Style(italic: true),
            new Style(strikethrough: true),
            new Style(underline: true),
            new Style(underlineWaved: true),
            new Style(underlineDotted: true),
            new Style(underlineDashed: true),
            new Style(underlineDouble: true)
        });
    }

    private class TestSourceItem : ISourceItem
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

    private class TestSource : Source<Group, TestSourceItem>
    {
        public List<TestSourceItem> Items { get; } = new();

        protected override void Custom()
        {
            Set(Group.GroupA, new Style(fg: "019ABF"));
            Set(Group.GroupB, bg: "019ABF");
            Link(Group.GroupC, Group.GroupA);
        }

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
    }

    private class SetTestSource : Source<Group, TestSourceItem>
    {
        public List<Style> Styles { get; } = new();

        protected override void Custom()
        {
            Set(Group.GroupA, fg: "1a09bf");
            Set(Group.GroupA, bg: "1a09bf");
            Set(Group.GroupA, sp: "1a09bf");
            Set(Group.GroupA, none: true);
            Set(Group.GroupA, bold: true);
            Set(Group.GroupA, italic: true);
            Set(Group.GroupA, strikethrough: true);
            Set(Group.GroupA, underline: true);
            Set(Group.GroupA, underlineWaved: true);
            Set(Group.GroupA, underlineDotted: true);
            Set(Group.GroupA, underlineDashed: true);
            Set(Group.GroupA, underlineDouble: true);
        }

        protected override void Set(Group group, Style style)
        {
            Styles.Add(style);
        }

        public override string Name => throw new NotImplementedException();

        protected override void Link(Group from, Group to)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<TestSourceItem> CollectItems()
        {
            throw new NotImplementedException();
        }
    }

    private enum Group
    {
        GroupA,
        GroupB,
        GroupC
    }
}