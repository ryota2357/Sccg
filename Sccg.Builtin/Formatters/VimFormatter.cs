// Links:
//   - :h :highlight (https://github.com/vim/vim/blob/master/runtime/doc/syntax.txt#L4984)
//   - :h attr-list (https://github.com/vim/vim/blob/master/runtime/doc/syntax.txt#L5067)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sccg.Builtin.Sources.Internal;
using Sccg.Builtin.Writers;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

public interface IVimSourceItem : IVimSourceItemBase
{
    public VimFormatter.Formattable Extract();
}

public interface IVimArrayVariableSourceItem : IVimSourceItemBase
{
    public VimFormatter.FormattableArrayVariable Extract();
}

public class VimFormatter : Formatter<IVimSourceItemBase, SingleTextContent>
{
    public override string Name => "Vim";

    protected override SingleTextContent Format(IEnumerable<IVimSourceItemBase> items, BuilderQuery query)
    {
        var metadata = query.GetMetadata();
        var header = CreateHeader(metadata).Select(x => x is null ? "" : $"\" {x}");
        var footer = CreateFooter(metadata).Select(x => x is null ? "" : $"\" {x}");
        var body = CreateBody(items.ToArray());

        return new SingleTextContent($"colors/{metadata.ThemeName}.vim",
            string.Join('\n', header),
            $"""
            set background={(metadata.ThemeType == ThemeType.Light ? "light" : "dark")}
            highlight clear
            if exists('syntax_on')
              syntax reset
            endif
            set t_Co=256
            let g:colors_name = '{metadata.ThemeName ?? "sccg_default"}'
            """,
            string.Join('\n', body),
            string.Join('\n', footer)
        );
    }

    private static IEnumerable<string> CreateBody(IVimSourceItemBase[] items)
    {
        var sb = new StringBuilder();

        void Set<T>(string name, T? value)
        {
            if (value is null) return;
            switch (value)
            {
                case Color c:
                    if (c.IsDefault) return;
                    var code = c.IsNone ? "NONE" : c.HexCode;
                    sb.Append($" {name}={code}");
                    break;
                case string s:
                    sb.Append($" {name}={s}");
                    break;
                default:
                    throw new NotSupportedException($"NeovimFormatter does not support type {typeof(T).Name}");
            }
        }

        // :hi
        foreach (var item in items.OfType<IVimSourceItem>())
        {
            var formattable = item.Extract();

            if (formattable.Link is not null)
            {
                yield return $"hi link {formattable.Name} {formattable.Link}";
                continue;
            }

            sb.Clear();
            sb.Append($"hi {formattable.Name}{(formattable.Default ? " default" : "")}");
            Set("ctermfg", formattable.Style?.Foreground.TerminalColorCode);
            Set("ctermbg", formattable.Style?.Background.TerminalColorCode);
            Set("ctermul", formattable.Style?.Special.TerminalColorCode);
            Set("cterm", CreateAttrList(formattable.Style?.Modifiers));
            Set("guifg", formattable.Style?.Foreground);
            Set("guibg", formattable.Style?.Background);
            Set("guisp", formattable.Style?.Special);
            Set("gui", CreateAttrList(formattable.Style?.Modifiers));

            yield return sb.ToString();
        }

        // let g:* = []
        var arrayItem = items.OfType<IVimArrayVariableSourceItem>()
                             .Select(x => x.Extract())
                             .GroupBy(x => x.Name);
        foreach (var one in arrayItem)
        {
            var name = one.Key;
            var data = one.OrderBy(x => x.Index)
                          .Select(x => (x.Value, x.Index, x.Length))
                          .ToArray();

            if (data.Select(x => x.Length).Distinct().Count() != 1)
            {
                throw new InvalidDataException($"Vim-array: {name} has different length.");
            }

            var idx = data.Select(x => x.Index).Distinct().ToArray();
            if (idx.Length != data.Length || idx.First() != 0 || idx.Last() != data.Length - 1)
            {
                throw new InvalidDataException("Vim-array: Index is not unique.");
            }

            var formattedValues = string.Join(", ", data.Select(x => $"'{x.Value}'"));
            yield return $"let g:{name} = [{formattedValues}]";
        }
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
            header = data.Select(x => $"{(x.Item1 + ":").PadRight(maxLen + 1, ' ')} {x.Item2}")
                         .ToArray();
        }
        else
        {
            header = Array.Empty<string>();
        }
        return header;
    }

    private static string?[] CreateFooter(Metadata metadata)
    {
        return metadata.Footer(metadata) ?? new[] { $"Built with Sccg {Metadata.__SccgVersion}" };
    }

    public readonly record struct Formattable(
        string Name,
        Style? Style,
        string? Link,
        bool Default = false
    );

    public readonly record struct FormattableArrayVariable(
        string Name,
        string Value,
        int Index,
        int Length
    );

    public static string? CreateAttrList(Style.Modifier? modifier)
    {
        return modifier?.DivideSingles()
                       .Select(x => x switch
                       {
                           Style.Modifier.Bold => "bold",
                           Style.Modifier.Italic => "italic",
                           Style.Modifier.Underline => "underline",
                           Style.Modifier.UnderlineWaved => "undercurl",
                           Style.Modifier.UnderlineDotted => "underdot",
                           Style.Modifier.UnderlineDashed => "underdash",
                           Style.Modifier.Strikethrough => "strikethrough",
                           Style.Modifier.None => "NONE",
                           _ => null
                       })
                       .WhereNotNull()
                       .DefaultIfEmpty(null)
                       .Aggregate((x, y) => $"{x},{y}");
    }
}