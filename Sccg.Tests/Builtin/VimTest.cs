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
}

file class EditorHighlight : VimEditorHighlightSource
{
    public override void Custom()
    {
        Set(Group.Conceal, fg: "1234af");
        Set(Group.lCursor, bg: "1234af");
        Set(Group.Pmenu, fg: "abc123", bold: true);
        Link(Group.Directory, Group.Conceal);
    }
}

file class SyntaxHighlight : VimSyntaxGroupSource
{
    public override void Custom()
    {
        Set(Group.PreCondit, fg: "ff0000", italic: true, bold: true);
        Set(Group.Comment, bg: "00ff00");
        Set(Group.Debug, none: true);
        Link(Group.Keyword, Group.PreCondit);
    }
}