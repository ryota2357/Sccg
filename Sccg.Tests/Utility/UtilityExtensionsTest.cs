using Sccg.Utility;

namespace Sccg.Tests.Utility;

public class UtilityExtensionsTest
{
    [Fact]
    public void WhereNotNull()
    {
        var res0 = UtilityExtensions.WhereNotNull<string?>(null);
        res0.Should().BeEmpty();

        var res1 = new int?[] { 1, null, 2, null, 3 }.WhereNotNull();
        res1.Should().BeEquivalentTo(new[] { 1, 2, 3 });

        var res2 = new[] { "a", null, "b", null, "c" }.WhereNotNull();
        res2.Should().BeEquivalentTo(new[] { "a", "b", "c" });

        var res3 = new[] { "a", "b", "c" }.WhereNotNull();
        res3.Should().BeEquivalentTo(new[] { "a", "b", "c" });
    }

    [Fact]
    public void StyleModifierContains()
    {
        var a = Style.Modifier.Bold | Style.Modifier.Italic;
        a.Contains(Style.Modifier.Bold).Should().BeTrue();
        a.Contains(Style.Modifier.Italic).Should().BeTrue();
        a.Contains(Style.Modifier.Underline).Should().BeFalse();

        var overwriteNone = Style.Modifier.None | Style.Modifier.Bold;
        overwriteNone.Contains(Style.Modifier.None).Should().BeFalse();
        overwriteNone.Contains(Style.Modifier.Bold).Should().BeTrue();

        var overwriteDefault = Style.Modifier.Default | Style.Modifier.Italic;
        overwriteDefault.Contains(Style.Modifier.Default).Should().BeFalse();
        overwriteDefault.Contains(Style.Modifier.Italic).Should().BeTrue();

        var none = Style.Modifier.Default | Style.Modifier.None;
        none.Contains(Style.Modifier.Default).Should().BeFalse();
        none.Contains(Style.Modifier.None).Should().BeTrue();

        var deft = Style.Modifier.Default;
        deft.Contains(Style.Modifier.Default).Should().BeTrue();
        deft.Contains(Style.Modifier.None).Should().BeFalse();
    }

    [Fact]
    public void StyleModifierDivideSingle()
    {
        var a = Style.Modifier.Bold | Style.Modifier.Italic;
        a.DivideSingles().Should().BeEquivalentTo(new[] { Style.Modifier.Bold, Style.Modifier.Italic });

        var overwriteNone = Style.Modifier.None | Style.Modifier.Bold;
        overwriteNone.DivideSingles().Should().BeEquivalentTo(new[] { Style.Modifier.Bold });

        var overwriteDefault = Style.Modifier.Default | Style.Modifier.Italic;
        overwriteDefault.DivideSingles().Should().BeEquivalentTo(new[] { Style.Modifier.Italic });

        var none = Style.Modifier.Default | Style.Modifier.None;
        none.DivideSingles().Should().BeEquivalentTo(new[] { Style.Modifier.None });

        var deft = Style.Modifier.Default;
        deft.DivideSingles().Should().BeEquivalentTo(new[] { Style.Modifier.Default });
    }
}