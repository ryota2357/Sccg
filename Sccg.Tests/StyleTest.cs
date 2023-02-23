namespace Sccg.Tests;

public class StyleTest
{
    [Fact]
    public void Construct()
    {
        var deft = new Style();
        Assert.Equal(Style.Default, deft);
        deft.Foreground.Should().Be(Color.Default);
        deft.Background.Should().Be(Color.Default);
        deft.Special.Should().Be(Color.Default);
        deft.Modifiers.Should().Be(Style.Modifier.Default);

        var normal = new Style(fg: "abcdef", bg: "012345", sp: "012FAB");
        normal.Foreground.Should().Be((Color)"abcdef");
        normal.Background.Should().Be((Color)"012345");
        normal.Special.Should().Be((Color)"012fab");
        normal.Modifiers.Should().Be(Style.Modifier.Default);

        var none = new Style(none: true);
        none.Modifiers.Should().Be(Style.Modifier.None);

        var bold = new Style(bold: true);
        var bold2 = Style.Default with { Bold = true };
        bold.Modifiers.Should().Be(Style.Modifier.Bold);
        bold2.Modifiers.Should().Be(Style.Modifier.Bold);

        var italic = new Style(italic: true);
        var italic2 = Style.Default with { Italic = true };
        italic.Modifiers.Should().Be(Style.Modifier.Italic);
        italic2.Modifiers.Should().Be(Style.Modifier.Italic);

        var underline = new Style(underline: true);
        var underline2 = Style.Default with { Underline = true };
        underline.Modifiers.Should().Be(Style.Modifier.Underline);
        underline2.Modifiers.Should().Be(Style.Modifier.Underline);

        var strikethrough = new Style(strikethrough: true);
        var strikethrough2 = Style.Default with { Strikethrough = true };
        strikethrough.Modifiers.Should().Be(Style.Modifier.Strikethrough);
        strikethrough2.Modifiers.Should().Be(Style.Modifier.Strikethrough);

        var underlineDashed = new Style(underlineDashed: true);
        var underlineDashed2 = Style.Default with { UnderlineDashed = true };
        underlineDashed.Modifiers.Should().Be(Style.Modifier.UnderlineDashed);
        underlineDashed2.Modifiers.Should().Be(Style.Modifier.UnderlineDashed);

        var underlineDotted = new Style(underlineDotted: true);
        var underlineDotted2 = Style.Default with { UnderlineDotted = true };
        underlineDotted.Modifiers.Should().Be(Style.Modifier.UnderlineDotted);
        underlineDotted2.Modifiers.Should().Be(Style.Modifier.UnderlineDotted);

        var underlineDouble = new Style(underlineDouble: true);
        var underlineDouble2 = Style.Default with { UnderlineDouble = true };
        underlineDouble.Modifiers.Should().Be(Style.Modifier.UnderlineDouble);
        underlineDouble2.Modifiers.Should().Be(Style.Modifier.UnderlineDouble);

        var underlineWaved = new Style(underlineWaved: true);
        var underlineWaved2 = Style.Default with { UnderlineWaved = true };
        underlineWaved.Modifiers.Should().Be(Style.Modifier.UnderlineWaved);
        underlineWaved2.Modifiers.Should().Be(Style.Modifier.UnderlineWaved);

        var marge = new Style(bold: true, italic: true);
        var marge2 = Style.Default with { Bold = true, Italic = true };
        marge.Modifiers.Should().Be(Style.Modifier.Bold | Style.Modifier.Italic);
        marge2.Modifiers.Should().Be(Style.Modifier.Bold | Style.Modifier.Italic);

        var erase = new Style(bold: true, underline: true) with { Bold = false };
        var erase2 = new Style(italic: true, underline: true) with { Bold = false, Italic = false};
        erase.Modifiers.Should().Be(Style.Modifier.Underline);
        erase2.Modifiers.Should().Be(Style.Modifier.Underline);
    }

    [Fact]
    public void Equal()
    {
        var a = new Style();
        var b = new Style();
        object c = new Style();
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals(c));

        var d = new Style(fg: "000");
        var e = new Style(fg: "000", bg: null);
        Assert.True(d == e);
    }

    [Fact]
    public void ToString_()
    {
        var deft = new Style();
        deft.ToString().Should().Be("Style(fg:default,bg:default,sp:default,default)");

        var normal = new Style(fg: "abcdef", bg: "012345", sp: "012FAB");
        normal.ToString().Should().Be("Style(fg:#abcdef,bg:#012345,sp:#012fab,default)");

        var none = new Style(none: true);
        none.ToString().Should().Be("Style(fg:default,bg:default,sp:default,none)");

        var underlineDouble = new Style(underlineDouble: true);
        underlineDouble.ToString().Should().Be("Style(fg:default,bg:default,sp:default,underline_double)");
    }
}