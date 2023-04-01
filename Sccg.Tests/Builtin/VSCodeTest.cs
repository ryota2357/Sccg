using Sccg.Builtin.Converters;
using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;

namespace Sccg.Tests.Builtin;

public class VSCodeTest
{
    private readonly Builder _builder;
    private readonly TestWriter _writer;

    public VSCodeTest()
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
        _builder.Use<MultiTextContentSplitter>();
        _builder.Use<VSCodeFormatter>();
    }

    [Fact]
    public void Test()
    {
        _builder.Use<EditorColor>();
        _builder.Build();

        _writer.Output.Should().HaveCount(2);

        var packageJson = _writer.Output[0];
        packageJson.Should().Be("""
        {
          "name": "unknown",
          "version": "1.0.0",
          "publisher": "ryota2357",
          "engines": {
            "vscode": "*"
          }
        }
        """);

        var themeJson = _writer.Output[1];
        themeJson.Should().Be("""
        {
          "name": "unknown",
          "type": "dark",
          "colors": {
            "foreground": "#ffffff",
            "widget.shadow": "#011223",
            "button.background": "#000000",
            "checkbox.background": "#000000"
          },
          "tokenColors": [
          ]
        }
        """);
    }
}

file class EditorColor : VSCodeEditorThemeColorSource
{
    protected override void Custom()
    {
        Set(Group.Base.Foreground, "#ffffff");
        Set(Group.Base.WidgetShadow, "#011223");
        Set(Group.Button.Background, "#000");
        Link(Group.Button.CheckboxBackground, Group.Button.Background);
    }
}