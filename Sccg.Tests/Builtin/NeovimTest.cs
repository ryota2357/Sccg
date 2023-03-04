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
              set background=dark
              highlight clear
              if exists('syntax_on')
                syntax reset
              endif
              set t_Co=256
            ]]
            vim.g.colors_name = 'sccg_default'
            vim.api.nvim_set_hl(0, 'lCursor', { fg = '#aaff00', bold = true, cterm = { bold = true } })
            vim.api.nvim_set_hl(0, 'Title', { bg = '#000000' })
            vim.api.nvim_set_hl(0, 'Menu', { cterm = {} })
            vim.api.nvim_set_hl(0, 'ColorColumn', { bold = true, underline = true, cterm = { bold = true, underline = true } })
            vim.api.nvim_set_hl(0, 'Conceal', { link = 'ColorColumn' })
            vim.api.nvim_set_hl(0, '@attribute', { fg = '#ff0000' })
            vim.api.nvim_set_hl(0, '@keyword.function', { bg = '#fffac9', italic = true, cterm = { italic = true } })
            vim.api.nvim_set_hl(0, '@text.environment.name', { bold = true, underline = true, cterm = { bold = true, underline = true } })
            vim.api.nvim_set_hl(0, '@character', { link = '@keyword.function' })
            vim.api.nvim_set_hl(0, '@keyword.function.test', { fg = '#ff0000' })
            vim.api.nvim_set_hl(0, '@boolean.test', { bg = '#fffac9', italic = true, cterm = { italic = true } })
            vim.api.nvim_set_hl(0, '@comment.test', { link = '@character' })
            vim.api.nvim_set_hl(0, '@constant', { })
            vim.api.nvim_set_hl(0, '@conceal', { link = '@boolean.test' })
            -- Built with Sccg
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
            -- Version:     1.0.0
            -- Author:      ryota2357
            -- Last change: 2023-01-23 Monday
            vim.cmd [[
              set background=dark
              highlight clear
              if exists('syntax_on')
                syntax reset
              endif
              set t_Co=256
            ]]
            vim.g.colors_name = 'sccg_default'
            vim.g.terminal_ansi_colors_0 = '#073642'
            vim.g.terminal_ansi_colors_2 = '#859900'
            vim.g.terminal_ansi_colors_3 = '#b58900'
            vim.g.terminal_ansi_colors_7 = '#eee8d5'
            vim.g.terminal_ansi_colors_8 = '#002b36'
            vim.g.terminal_ansi_colors_10 = '#586e75'
            vim.g.terminal_ansi_colors_15 = '#fdf6e3'
            -- Built with Sccg
            """
        );
    }
}

file class EditorHighlight : NeovimEditorHighlightSource
{
    protected override void Custom()
    {
        Set(Group.lCursor, fg: "aaff00", bold: true);
        Set(Group.Title, bg: "000000");
        Set(Group.Menu, none: true);
        Set(Group.ColorColumn, underline: true, bold: true);
        Link(Group.Conceal, Group.ColorColumn);
    }
}

file class TreesitterHighlight : NeovimTreesitterHighlightSource
{
    protected override void Custom()
    {
        Set(Group.Attribute, fg: "ff0000");
        Set(Group.KeywordFunction, bg: "fffac9", italic: true);
        Set(Group.TextEnvironmentName, bold: true, underline: true);
        Link(Group.Character, Group.KeywordFunction);

        Filetype("test", () =>
        {
            Set(Group.KeywordFunction, fg: "ff0000");
            Set(Group.Boolean, bg: "fffac9", italic: true);
            Link(Group.Comment, Group.Character);
        });

        Set(Group.Constant, Style.Default);
        Link(Group.Conceal, Group.Boolean, "test");
    }
}

file class Ansi16Color : Ansi16ColorSource
{
    protected override void Custom()
    {
        Set(Group.Ansi0, "#073642");
        Set(Group.Ansi2, "#859900");
        Set(Group.Ansi3, "#b58900");
        Set(Group.Ansi7, "#eee8d5");
        Set(Group.Ansi8, "#002b36");
        Set(Group.Ansi10, "#586e75");
        Set(Group.Ansi15, "#fdf6e3");
    }
}