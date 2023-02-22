using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;

namespace Sccg.Tests.Builtin;

public class VimTest
{
    private readonly Builder _builder;
    private readonly TestWriter _writer;

    public VimTest()
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
        _builder.Use<VimFormatter>();
    }

    [Fact]
    public void Test()
    {
        _builder.Use<EditorHighlight>();
        _builder.Use<SyntaxHighlight>();
        _builder.Build();

        _writer.Output.Should().HaveCount(1);
        var output = _writer.Output.First();
        output.Should().Be(
            """
            " Version:     1.0.0
            " Author:      ryota2357
            " Last change: 2023-01-23 Monday
            hi clear
            if exists('syntax_on')
              syntax reset
            endif
            let g:colors_name = 'sccg_default'
            hi Conceal guifg=#1234af
            hi lCursor guibg=#1234af
            hi Pmenu cterm=bold guifg=#abc123 gui=bold
            hi link Directory Conceal
            hi PreCondit cterm=bold,italic guifg=#ff0000 gui=bold,italic
            hi Comment guibg=#00ff00
            hi Debug cterm=NONE gui=NONE
            hi link Keyword PreCondit
            " Built with Sccg
            """
        );
    }

    [Fact]
    public void Ansi16Test()
    {
        _builder.Use<Ansi16Color>();
        _builder.Build();

        _writer.Output.Should().HaveCount(1);
        var output = _writer.Output.First();
        output.Should().Be(
            """
            " Version:     1.0.0
            " Author:      ryota2357
            " Last change: 2023-01-23 Monday
            hi clear
            if exists('syntax_on')
              syntax reset
            endif
            let g:colors_name = 'sccg_default'
            let g:terminal_ansi_colors = ['#073642', '#dc322f', '#859900', '#b58900', '#268bd2', '#d33682', '#2aa198', '#eee8d5', '#002b36', '#cb4b16', '#586e75', '#657b83', '#839496', '#6c71c4', '#93a1a1', '#fdf6e3']
            " Built with Sccg
            """
        );
    }

    [Fact]
    public void Ansi16FailedTest()
    {
        _builder.Use<InvalidAnsi16Color>();
        Assert.Throws<Exception>(() => _builder.Build());
    }
}

file class EditorHighlight : VimEditorHighlightSource
{
    protected override void Custom()
    {
        Set(Group.Conceal, fg: "1234af");
        Set(Group.lCursor, bg: "1234af");
        Set(Group.Pmenu, fg: "abc123", bold: true);
        Link(Group.Directory, Group.Conceal);
    }
}

file class SyntaxHighlight : VimSyntaxGroupSource
{
    protected override void Custom()
    {
        Set(Group.PreCondit, fg: "ff0000", italic: true, bold: true);
        Set(Group.Comment, bg: "00ff00");
        Set(Group.Debug, none: true);
        Link(Group.Keyword, Group.PreCondit);
    }
}

file class Ansi16Color : Ansi16ColorSource
{
    protected override void Custom()
    {
        Set(Group.Ansi0, "#073642");
        Set(Group.Ansi1, "#dc322f");
        Set(Group.Ansi2, "#859900");
        Set(Group.Ansi3, "#b58900");
        Set(Group.Ansi4, "#268bd2");
        Set(Group.Ansi5, "#d33682");
        Set(Group.Ansi6, "#2aa198");
        Set(Group.Ansi7, "#eee8d5");
        Set(Group.Ansi8, "#002b36");
        Set(Group.Ansi9, "#cb4b16");
        Set(Group.Ansi10, "#586e75");
        Set(Group.Ansi11, "#657b83");
        Set(Group.Ansi12, "#839496");
        Set(Group.Ansi13, "#6c71c4");
        Set(Group.Ansi14, "#93a1a1");
        Set(Group.Ansi15, "#fdf6e3");
    }
}

file class InvalidAnsi16Color : Ansi16ColorSource
{
    protected override void Custom()
    {
        Set(Group.Ansi0, "#073642");
        Set(Group.Ansi1, "#dc322f");
        Set(Group.Ansi4, "#268bd2");
        Set(Group.Ansi5, "#d33682");
        Set(Group.Ansi8, "#002b36");
        Set(Group.Ansi14, "#93a1a1");
        Set(Group.Ansi15, "#fdf6e3");
    }
}