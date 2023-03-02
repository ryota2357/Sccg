// Links:
//   - https://neovim.io/doc/user/api.html#nvim_set_hl()
//   - :h nvim_set_hl() (https://github.com/neovim/neovim/blob/master/runtime/doc/api.txt#L1379)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Sources.Internal;
using Sccg.Builtin.Writers;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

/// <summary>
/// SourceItem for Neovim color scheme.
/// </summary>
public interface INeovimSourceItem : INeovimSourceItemBase
{
    public NeovimFormatter.Formattable Extract();
}

/// <summary>
/// SourceItem for Neovim color scheme.
/// </summary>
public interface INeovimVariableSourceItem : INeovimSourceItemBase
{
    public NeovimFormatter.FormattableVariable? Extract();
}

/// <summary>
/// Neovim color scheme with Lua.
/// </summary>
public class NeovimFormatter : Formatter<INeovimSourceItemBase, SingleTextContent>
{
    public override string Name => "Neovim";

    /// <inheritdoc />
    protected override SingleTextContent Format(IEnumerable<INeovimSourceItemBase> items, BuilderQuery query)
    {
        var metadata = query.GetMetadata();
        var header = StdFormatterImpl.CreateHeader(metadata, "--");
        var footer = StdFormatterImpl.CreateFooter(metadata, "--");
        var body = CreateBody(items.ToArray());

        return new SingleTextContent($"colors/{metadata.ThemeName}.lua",
            string.Join('\n', header),
            $"""
            vim.cmd [[
              set background={(metadata.ThemeType == ThemeType.Light ? "light" : "dark")}
              highlight clear
              if exists('syntax_on')
                syntax reset
              endif
              set t_Co=256
            ]]
            vim.g.colors_name = '{metadata.ThemeName ?? "sccg_default"}'
            """,
            string.Join("\n", body),
            string.Join('\n', footer)
        );
    }

    private static IEnumerable<string> CreateBody(INeovimSourceItemBase[] items)
    {
        var sb = new StringBuilder();

        void Set<T>(string name, T? value)
        {
            if (value is null) return;
            switch (value)
            {
                case bool b:
                    // NOTE: Because of using nvim_set_hl(), don't have to set false value. `false` is same as omitting.
                    if (!b) return;
                    sb.Append($" {name} = true,");
                    break;
                case Color c:
                    if (c.IsDefault) return;
                    var code = c.IsNone ? "NONE" : c.HexCode;
                    sb.Append($" {name} = '{code}',");
                    break;
                case int i:
                    sb.Append($" {name} = {i},");
                    break;
                case string s:
                    sb.Append($" {name} = '{s}',");
                    break;
                default:
                    throw new NotSupportedException($"NeovimFormatter does not support type {typeof(T).Name}");
            }
        }

        // vim.api.nvim_set_hl
        foreach (var item in items.OfType<INeovimSourceItem>())
        {
            var formattable = item.Extract();

            sb.Clear();
            sb.Append($"vim.api.nvim_set_hl({formattable.Id}, '{formattable.Name}', {{");
            if (formattable.Link is not null)
            {
                sb.Append($" link = '{formattable.Link}'");
            }
            else
            {
                Set("fg", formattable.Style?.Foreground);
                Set("bg", formattable.Style?.Background);
                Set("sp", formattable.Style?.Special);
                Set("blend", formattable.Blend);
                Set("bold", formattable.Style?.Modifiers.Contains(Style.Modifier.Bold));
                Set("standout", formattable.Standout);
                Set("underline", formattable.Style?.Modifiers.Contains(Style.Modifier.Underline));
                Set("undercurl", formattable.Style?.Modifiers.Contains(Style.Modifier.UnderlineWaved));
                Set("underdouble", formattable.Style?.Modifiers.Contains(Style.Modifier.UnderlineDouble));
                Set("underdotted", formattable.Style?.Modifiers.Contains(Style.Modifier.UnderlineDotted));
                Set("underdashed", formattable.Style?.Modifiers.Contains(Style.Modifier.UnderlineDashed));
                Set("strikethrough", formattable.Style?.Modifiers.Contains(Style.Modifier.Strikethrough));
                Set("italic", formattable.Style?.Modifiers.Contains(Style.Modifier.Italic));
                Set("reverse", formattable.Reverse);
                Set("nocombine", formattable.Nocombine);
                Set("default", formattable.Default);
                Set("ctermfg", formattable.Style?.Foreground.TerminalColorCode);
                Set("ctermbg", formattable.Style?.Background.TerminalColorCode);

                if (formattable.Style?.Modifiers.Contains(Style.Modifier.Default) == false)
                {
                    sb.Append(" cterm = {");
                    var ctermListStr = VimFormatter.CreateAttrList(formattable.Style?.Modifiers);
                    if (ctermListStr is not null && ctermListStr != "NONE")
                    {
                        var ctermList = ctermListStr.Split(",").Select(x => $"{x} = true");
                        sb.Append(' ');
                        sb.Append(ctermList.Aggregate((a, b) => $"{a}, {b}"));
                        sb.Append(' ');
                    }
                    sb.Append('}');
                }
            }

            var str = sb.ToString();
            if (str.EndsWith(","))
            {
                str = str[..^1];
            }
            yield return $"{str} }})";
        }

        // vim.g.* = { *, *, ...}
        var variableItem = items.OfType<INeovimVariableSourceItem>()
                                .Select(x => x.Extract())
                                .WhereNotNull();
        foreach (var (name, value) in variableItem)
        {
            yield return $"vim.g.{name} = '{value}'";
        }
    }

    public readonly record struct Formattable(
        int Id,
        string Name,
        Style? Style,
        string? Link,
        bool? Standout,
        bool? Reverse,
        bool? Nocombine,
        bool? Default
    )
    {
        private readonly int? _blend = null;

        public int? Blend
        {
            get => _blend;
            init =>
                _blend = value switch
                {
                    < 0 or > 100 => throw new ArgumentOutOfRangeException(nameof(value),
                        "Blend must be between 0 and 100"),
                    _ => value
                };
        }
    }

    public readonly record struct FormattableVariable(
        string Name,
        string Value
    );
}