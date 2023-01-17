using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;

namespace Sccg.Tests.Builtin;

public class Iterm2Test
{
    private readonly Builder _builder;
    private readonly TestContentWriter _writer;

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
        _writer = new TestContentWriter();

        _builder.Use(_writer);
        _builder.Use<Iterm2Formatter>();
    }

    [Fact]
    public void Test()
    {
        _builder.Use<Colors>();
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
    public override void Custom()
    {
        Set(Group.Ansi0, "000000");
        Set(Group.Ansi10, "#32C81E");
        Link(Group.Background, Group.Ansi0);
        Set(Group.CursorGuide, "f1fff0");
    }
}