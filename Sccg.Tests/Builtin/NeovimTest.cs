using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;

namespace Sccg.Tests.Builtin;

public class NeovimTest
{
    private readonly Builder _builder;
    private readonly TestWriter _writer;

    public NeovimTest()
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
        _builder.Use<NeovimFormatter>();
    }

    [Fact]
    public void Test()
    {
        _builder.Use<EditorHighlight>();
        _builder.Use<TreesitterHighlight>();
        _builder.Build();

        _writer.Output.Should().HaveCount(1);
        var output = _writer.Output.First();
        output.Should().Be(
            """
            -- Version:     1.0.0
            -- Author:      ryota2357
            -- Last change: 2023-01-23 Monday
            vim.cmd [[
              highlight clear
              if exists('syntax_on')
                syntax reset
              endif
            ]]
            vim.g.colors_name = 'sccg_default'
            vim.api.nvim_set_hl(0, 'lCursor', { fg = '#aaff00', bold = true, cterm = 'bold' })
            vim.api.nvim_set_hl(0, 'Title', { bg = '#000000' })
            vim.api.nvim_set_hl(0, 'ColorColumn', { bold = true, underline = true, cterm = 'bold,underline' })
            vim.api.nvim_set_hl(0, 'Conceal', { link = 'ColorColumn' })
            vim.api.nvim_set_hl(0, '@attribute', { fg = '#ff0000' })
            vim.api.nvim_set_hl(0, '@keyword.function', { bg = '#fffac9', italic = true, cterm = 'italic' })
            vim.api.nvim_set_hl(0, '@text.environment.name', { bold = true, underline = true, cterm = 'bold,underline' })
            vim.api.nvim_set_hl(0, '@character', { link = '@keyword.function' })
            -- Built with Sccg
            """
        );
    }
}

file class EditorHighlight : NeovimEditorHighlightSource
{
    public override void Custom()
    {
        Set(Group.lCursor, fg: "aaff00", bold: true);
        Set(Group.Title, bg: "000000");
        Set(Group.ColorColumn, underline: true, bold: true);
        Link(Group.Conceal, Group.ColorColumn);
    }
}

file class TreesitterHighlight : NeovimTreesitterHighlightSource
{
    public override void Custom()
    {
        Set(Group.Attribute, fg: "ff0000");
        Set(Group.KeywordFunction, bg: "fffac9", italic: true);
        Set(Group.TextEnvironmentName, bold: true, underline: true);
        Link(Group.Character, Group.KeywordFunction);
    }
}