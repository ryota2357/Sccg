using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;

namespace Sccg.Tests.Builtin;

public class Iterm2Test
{
    private readonly Builder _builder;
    private readonly TestWriter _writer;

    public Iterm2Test()
    {
        _builder = new Builder
        {
            Metadata = Metadata.Default with
            {
                Author = "ryota2357",
                LastUpdated = DateTime.Parse("2023-01-23T01:23:45"),
                Footer = _ => new[] { "Built with Sccg" }
            }
        };
        _writer = new TestWriter();

        _builder.Use(_writer);
        _builder.Use<Iterm2Formatter>();
    }

    [Fact]
    public void Test()
    {
        _builder.Use<Colors>();
        _builder.Use<Ansi16Colors>();
        _builder.Build();

        _writer.Output.Should().HaveCount(1);
        var output = _writer.Output.First();
        output.Should().Be(
            $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
            <dict>
            {'\t'}<key>Ansi 0 Color</key>
            {'\t'}<dict>
            {'\t'}{'\t'}<key>Color Space</key>
            {'\t'}{'\t'}<string>sRGB</string>
            {'\t'}{'\t'}<key>Alpha Component</key>
            {'\t'}{'\t'}<real>1</real>
            {'\t'}{'\t'}<key>Red Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}{'\t'}<key>Green Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}{'\t'}<key>Blue Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}</dict>
            {'\t'}<key>Ansi 8 Color</key>
            {'\t'}<dict>
            {'\t'}{'\t'}<key>Color Space</key>
            {'\t'}{'\t'}<string>sRGB</string>
            {'\t'}{'\t'}<key>Alpha Component</key>
            {'\t'}{'\t'}<real>1</real>
            {'\t'}{'\t'}<key>Red Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}{'\t'}<key>Green Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}{'\t'}<key>Blue Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}</dict>
            {'\t'}<key>Ansi 10 Color</key>
            {'\t'}<dict>
            {'\t'}{'\t'}<key>Color Space</key>
            {'\t'}{'\t'}<string>sRGB</string>
            {'\t'}{'\t'}<key>Alpha Component</key>
            {'\t'}{'\t'}<real>1</real>
            {'\t'}{'\t'}<key>Red Component</key>
            {'\t'}{'\t'}<real>{(50.0 / 255.0):F16}</real>
            {'\t'}{'\t'}<key>Green Component</key>
            {'\t'}{'\t'}<real>{(200.0 / 255.0):F16}</real>
            {'\t'}{'\t'}<key>Blue Component</key>
            {'\t'}{'\t'}<real>{(30.0 / 255.0):F16}</real>
            {'\t'}</dict>
            {'\t'}<key>Background Color</key>
            {'\t'}<dict>
            {'\t'}{'\t'}<key>Color Space</key>
            {'\t'}{'\t'}<string>sRGB</string>
            {'\t'}{'\t'}<key>Alpha Component</key>
            {'\t'}{'\t'}<real>1</real>
            {'\t'}{'\t'}<key>Red Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}{'\t'}<key>Green Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}{'\t'}<key>Blue Component</key>
            {'\t'}{'\t'}<real>0.0000000000000000</real>
            {'\t'}</dict>
            {'\t'}<key>Cursor Guide Color</key>
            {'\t'}<dict>
            {'\t'}{'\t'}<key>Color Space</key>
            {'\t'}{'\t'}<string>sRGB</string>
            {'\t'}{'\t'}<key>Alpha Component</key>
            {'\t'}{'\t'}<real>1</real>
            {'\t'}{'\t'}<key>Red Component</key>
            {'\t'}{'\t'}<real>{(241.0 / 255.0):F16}</real>
            {'\t'}{'\t'}<key>Green Component</key>
            {'\t'}{'\t'}<real>1.0000000000000000</real>
            {'\t'}{'\t'}<key>Blue Component</key>
            {'\t'}{'\t'}<real>{(240.0 / 255.0):F16}</real>
            {'\t'}</dict>
            </dict>
            </plist>
            """
        );
    }
}

file class Colors : Iterm2ColorsSource
{
    protected override void Custom(BuilderQuery query)
    {
        var ansi16Colors = query.GetSourceItems<Ansi16ColorSource.Item>();
        var ansi8 = ansi16Colors.Where(x => x.IsAnsi(8))
                                .Select(x => x.Color)
                                .FirstOrDefault("#123456");
        var ansi10 = ansi16Colors.Where(x => x.IsAnsi(10))
                                 .Select(x => x.Color)
                                 .FirstOrDefault("#32C81E");
        Set(Group.Ansi0, "000000");
        Set(Group.Ansi8, ansi8); // #000000
        Set(Group.Ansi10, ansi10); // #32C81E
        Link(Group.Background, Group.Ansi0);
        Set(Group.CursorGuide, "f1fff0");
    }
}

file class Ansi16Colors : Ansi16ColorSource
{
    protected override Target ItemTarget => Target.None;

    protected override void Custom()
    {
        Set(Group.Ansi0, "000000");
        Link(Group.Ansi8, Group.Ansi0);
    }
}