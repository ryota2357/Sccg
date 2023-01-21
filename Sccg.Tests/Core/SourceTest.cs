using Sccg.Core;

namespace Sccg.Tests.Core;

public class SourceTest
{
    [Fact]
    public void Test()
    {
        var query = BuilderQuery.Empty;

        ISource source = new TestSource();
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
        var query = BuilderQuery.Empty;

        var source = new SetTestSource();
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
}

file class SetTestSource : Source<TestSourceBase.Group, TestSourceItem>
{
    public List<Style> Styles { get; } = new();

    protected override void Custom()
    {
        Set(TestSourceBase.Group.GroupA, fg: "1a09bf");
        Set(TestSourceBase.Group.GroupA, bg: "1a09bf");
        Set(TestSourceBase.Group.GroupA, sp: "1a09bf");
        Set(TestSourceBase.Group.GroupA, none: true);
        Set(TestSourceBase.Group.GroupA, bold: true);
        Set(TestSourceBase.Group.GroupA, italic: true);
        Set(TestSourceBase.Group.GroupA, strikethrough: true);
        Set(TestSourceBase.Group.GroupA, underline: true);
        Set(TestSourceBase.Group.GroupA, underlineWaved: true);
        Set(TestSourceBase.Group.GroupA, underlineDotted: true);
        Set(TestSourceBase.Group.GroupA, underlineDashed: true);
        Set(TestSourceBase.Group.GroupA, underlineDouble: true);
    }

    protected override void Set(TestSourceBase.Group group, Style style)
    {
        Styles.Add(style);
    }

    public override string Name => throw new NotImplementedException();

    protected override void Link(TestSourceBase.Group from, TestSourceBase.Group to)
    {
        throw new NotImplementedException();
    }
    protected override IEnumerable<TestSourceItem> CollectItems()
    {
        throw new NotImplementedException();
    }
}