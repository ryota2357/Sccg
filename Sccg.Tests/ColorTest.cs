namespace Sccg.Tests;

public class ColorTest
{
    [Fact]
    public void Construct()
    {
        var a = new Color("810af1");
        var b = new Color("810Af1");
        var c = new Color("#810Af1");
        var d = new Color(129, 10, 241);
        var e = new Color("09f");
        var f = new Color("09F");
        var g = new Color("#09f");
        var h = new Color("810af1", 17);
        var i = new Color("#09f", 17);
        var j = new Color(129, 10, 241, 17);

        a.HexCode.Should().Be("#810af1");
        b.HexCode.Should().Be("#810af1");
        c.HexCode.Should().Be("#810af1");
        d.HexCode.Should().Be("#810af1");
        e.HexCode.Should().Be("#0099ff");
        f.HexCode.Should().Be("#0099ff");
        g.HexCode.Should().Be("#0099ff");
        h.HexCode.Should().Be("#810af1");
        i.HexCode.Should().Be("#0099ff");
        j.HexCode.Should().Be("#810af1");
        a.TerminalColorCode.Should().BeNull();
        b.TerminalColorCode.Should().BeNull();
        c.TerminalColorCode.Should().BeNull();
        d.TerminalColorCode.Should().BeNull();
        e.TerminalColorCode.Should().BeNull();
        f.TerminalColorCode.Should().BeNull();
        g.TerminalColorCode.Should().BeNull();
        h.TerminalColorCode.Should().Be(17);
        i.TerminalColorCode.Should().Be(17);
        j.TerminalColorCode.Should().Be(17);

        Assert.Throws<ArgumentException>(() => new Color("0abcdef"));
        Assert.Throws<ArgumentException>(() => new Color("012efg"));
        Assert.Throws<ArgumentException>(() => new Color("0123"));
        Assert.Throws<ArgumentException>(() => new Color("01g"));
    }

    [Fact]
    public void Convert()
    {
        var a = (Color)"ab0123";
        var b = (Color)"AB0123";
        var c = (Color)"#AB0123";
        var d = (Color)(171, 1, 35);

        a.HexCode.Should().Be("#ab0123");
        b.HexCode.Should().Be("#ab0123");
        c.HexCode.Should().Be("#ab0123");
        d.HexCode.Should().Be("#ab0123");
        a.TerminalColorCode.Should().BeNull();
        b.TerminalColorCode.Should().BeNull();
        c.TerminalColorCode.Should().BeNull();
        d.TerminalColorCode.Should().BeNull();
    }

    [Fact]
    public void Constant()
    {
        var deft = Color.Default;
        var none = Color.None;

        Assert.True(deft == Color.Default);
        Assert.True(none == Color.None);
        Assert.Equal(Color.Default, deft.HexCode);
        Assert.Equal(Color.None, none.HexCode);
        Assert.False(deft == none);

        deft.IsDefault().Should().BeTrue();
        deft.IsNone().Should().BeFalse();
        none.IsDefault().Should().BeFalse();
        none.IsNone().Should().BeTrue();
    }

    [Fact]
    public void AlphaBlend()
    {
        var fg = new Color("#8000ff");
        var bg = new Color("#ff8000");
        Color.AlphaBlend(fg, bg, 0.5f).HexCode.Should().Be("#c04080");
        Color.AlphaBlend(fg, bg, 0.0f).HexCode.Should().Be("#ff8000");
        Color.AlphaBlend(fg, bg, 1.0f).HexCode.Should().Be("#8000ff");

        Assert.Throws<ArgumentException>(() => Color.AlphaBlend(Color.None, bg, 1));
        Assert.Throws<ArgumentException>(() => Color.AlphaBlend(Color.Default, bg, 1));
        Assert.Throws<ArgumentException>(() => Color.AlphaBlend(fg, Color.None, 1));
        Assert.Throws<ArgumentException>(() => Color.AlphaBlend(fg, Color.Default, 1));
    }

    [Fact]
    public void Equal()
    {
        var a = new Color("1234Ab");
        var b = new Color("1234AB", 3);
        object c = new Color("1234AB", 3);
        Assert.True(a == b);
        Assert.True(a == "1234ab");
        Assert.True(b == "#1234ab");
        Assert.False(a != b);

        Assert.True(a.Equals(c));
        Assert.True(b.Equals(c));
        Assert.False(b.Equals(new object()));
    }

    [Fact]
    public void HashCode()
    {
        var a = new Color("#000000");
        a.GetHashCode().Should().Be(0);

        var b = new Color("a01f01");
        var br = (16 * 10) << 16;
        var bg = (16 * 1 + 15) << 8;
        var bb = (16 * 0 + 1) << 0;
        b.GetHashCode().Should().Be(br + bg + bb);

        var c = Color.Default;
        c.GetHashCode().Should().Be(-1);

        var d = Color.None;
        d.GetHashCode().Should().Be(-2);
    }

    [Fact]
    public void ToString_()
    {
        var a = new Color("1234Ab");
        a.ToString().Should().Be("Color(#1234ab)");

        var b = new Color("1234Ab", 17);
        b.ToString().Should().Be("Color(#1234ab,17)");

        var c = Color.Default;
        c.ToString().Should().Be("Color(default)");

        var d = Color.None;
        d.ToString().Should().Be("Color(none)");
    }
}