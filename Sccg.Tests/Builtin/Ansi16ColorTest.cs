using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Tests.Builtin;

public class Ansi16ColorTest
{
    private IEnumerable<ISourceItem> _ansi16;

    public Ansi16ColorTest()
    {
        var source = new Ansi16Color();
        source.CallCustom();
        _ansi16 = (source as ISource).CollectItems();
    }

    [Fact]
    public void Iterm2Test()
    {
        var source = new Iterm2Color();
        source.CallCustom();
        var items = (source as ISource).CollectItems()
                                       .OfType<IIterm2SourceItem>()
                                       .Select(x => x.Extract())
                                       .WhereNotNull()
                                       .ToArray();
        var expected = _ansi16.OfType<IIterm2SourceItem>()
                              .Select(x => x.Extract())
                              .WhereNotNull()
                              .ToArray();
        items.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AlacrittyTest()
    {
        var source = new AlacrittyColor();
        source.CallCustom();
        var items = (source as ISource).CollectItems()
                                       .OfType<IAlacrittySourceItem>()
                                       .Select(x => x.Extract())
                                       .WhereNotNull()
                                       .ToArray();
        var expected = _ansi16.OfType<IAlacrittySourceItem>()
                              .Select(x => x.Extract())
                              .WhereNotNull()
                              .ToArray();
        items.Should().BeEquivalentTo(expected);
    }
}

file interface ICallCustom
{
    void CallCustom();
}

file class Ansi16Color : Ansi16ColorSource, ICallCustom
{
    protected override void Custom()
    {
        Set(Group.Ansi0, "#000000");
        Set(Group.Ansi2, "#00ff00");
        Set(Group.Ansi10, "#32C81E");
        Set(Group.Ansi15, "#ffffff");
    }

    public void CallCustom() => Custom();
}

file class Iterm2Color : Iterm2ColorsSource, ICallCustom
{
    protected override void Custom()
    {
        Set(Group.Ansi0, "#000000");
        Set(Group.Ansi2, "#00ff00");
        Set(Group.Ansi10, "#32C81E");
        Set(Group.Ansi15, "#ffffff");
    }

    public void CallCustom() => Custom();
}

file class AlacrittyColor : AlacrittyColorsSource, ICallCustom
{
    protected override void Custom()
    {
        Set(Group.NormalBlack, "#000000");
        Set(Group.NormalGreen, "#00ff00");
        Set(Group.BrightGreen, "#32C81E");
        Set(Group.BrightWhite, "#ffffff");
    }

    public void CallCustom() => Custom();
}