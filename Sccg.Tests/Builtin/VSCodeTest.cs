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
        _builder.Use<TextMate>();
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
          },
          "categories": [
            "Themes"
          ],
          "contributes": {
            "themes": [
              {
                "label": "unknown",
                "uiTheme": "vs-dark",
                "path": "./themes/unknown-color-theme.json"
              }
            ]
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
            {
              "scope": [
                "comment",
                "comment.line.double-dash"
              ],
              "settings": {
                "foreground": "#987987"
              }
            },
            {
              "scope": [
                "keyword",
                "markup.other"
              ],
              "settings": {
                "foreground": "#0c22c8",
                "fontStyle": "bold"
              }
            },
            {
              "scope": [
                "keyword.operator",
                "keyword.control"
              ],
              "settings": {
                "foreground": "#0c22c8",
                "fontStyle": "bold italic"
              }
            },
            {
              "scope": "invalid",
              "settings": {
                "fontStyle": "strikethrough"
              }
            }
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

file class TextMate : TextMateElementSource
{
    protected override void Custom()
    {
        Set(Group.Comment, fg: "987987");
        Link(Group.CommentLineDouble_dash, Group.Comment);
        Set(Group.Keyword, fg: "#0C22C8", bold: true);
        Set(Group.KeywordOperator, fg: "#0C22C8", italic: true, bold: true);
        Set(Group.KeywordControl, fg: "#0C22C8", italic: true, bold: true);
        Set(Group.Invalid, strikethrough: true);
        Link(Group.MarkupOther, Group.Keyword);
    }
}