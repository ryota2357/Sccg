// Links:
//   - https://neovim.io/doc/user/api.html#nvim_set_hl()
//   - :h nvim_set_hl() (https://github.com/neovim/neovim/blob/master/runtime/doc/api.txt#L1379)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sccg.Builtin.Sources.Internal;
using Sccg.Builtin.Writers;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

public interface INeovimSourceItem : INeovimSourceItemBase
{
    public NeovimFormatter.Formattable Extract();
}

public interface INeovimVariableSourceItem : INeovimSourceItemBase
{
    public NeovimFormatter.FormattableVariable Extract();
}

/// <summary>
/// Create a Neovim color scheme with Lua.
/// </summary>
public class NeovimFormatter : Formatter<INeovimSourceItemBase, SingleTextContent>
{
    public override string Name => "Neovim";

    protected override SingleTextContent Format(IEnumerable<INeovimSourceItemBase> items, BuilderQuery query)
    {
        var metadata = query.GetMetadata();
        var header = CreateHeader(metadata).Select(x => x is null ? "" : $"-- {x}");
        var footer = CreateFooter(metadata).Select(x => x is null ? "" : $"-- {x}");
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

    private static string?[] CreateHeader(Metadata metadata)
    {
        var header = metadata.Header(metadata);
        if (header is not null)
        {
            return header;
        }

        var data = new[]
            {
                ("Name", metadata.ThemeName),
                ("Version", metadata.Version),
                ("Author", metadata.Author),
                ("Maintainer", metadata.Maintainer),
                ("License", metadata.License),
                ("Description", metadata.Description),
                ("Homepage", metadata.Homepage),
                ("Repository", metadata.Repository),
                ("Last change",
                    metadata.LastUpdated?.ToString("yyyy-MM-dd dddd",
                        System.Globalization.CultureInfo.CreateSpecificCulture("en-US")))
            }.Where(x => x.Item2 is not null)
             .OfType<(string, string)>()
             .ToArray();
        if (data.Length != 0)
        {
            var maxLen = data.Max(x => x.Item1.Length);
            header = data.Select(x => $"{(x.Item1 + ":").PadRight(maxLen + 1, ' ')} {x.Item2}").ToArray();
        }
        else
        {
            header = Array.Empty<string>();
        }

        return header;
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
                    sb.Append($"{name} = true, ");
                    break;
                case Color c:
                    if (c.IsDefault) return;
                    var code = c.IsNone ? "NONE" : c.HexCode;
                    sb.Append($"{name} = '{code}', ");
                    break;
                case int i:
                    sb.Append($"{name} = {i}, ");
                    break;
                case string s:
                    sb.Append($"{name} = '{s}', ");
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
            sb.Append($"vim.api.nvim_set_hl({formattable.Id}, '{formattable.Name}', {{ ");
            if (formattable.Link is not null)
            {
                sb.Append($"link = '{formattable.Link}'");
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

                if (formattable.Style?.Modifiers.Contains(Style.Modifier.Default) == true)
                {
                    sb.Remove(sb.Length - 2, 2); // Remove last ", "
                }
                else
                {
                    sb.Append("cterm = {");
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
            sb.Append(" })");

            yield return sb.ToString();
        }

        // vim.g.* = { *, *, ...}
        var variableItem = items.OfType<INeovimVariableSourceItem>().Select(x => x.Extract());
        foreach (var (name, value) in variableItem)
        {
            yield return $"vim.g.{name} = '{value}'";
        }
    }

    private static string?[] CreateFooter(Metadata metadata)
    {
        return metadata.Footer(metadata) ?? new[] { $"Built with Sccg {Metadata.__SccgVersion}" };
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