// Links:
//   - :h :highlight (https://github.com/vim/vim/blob/master/runtime/doc/syntax.txt#L4984)
//   - :h attr-list (https://github.com/vim/vim/blob/master/runtime/doc/syntax.txt#L5067)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sccg.Builtin.ContentWriters;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

public interface IVimSourceItem : ISourceItem
{
    public VimFormatter.Formattable Extract();
}

public class VimFormatter : Formatter<IVimSourceItem, SingleTextContent>, IMetadataUser
{
    public Metadata Metadata { get; set; } = Metadata.Empty;

    public override string Name => "Vim";

    protected override SingleTextContent Format(IEnumerable<IVimSourceItem> items)
    {
        var header = Metadata.Header(Metadata);
        if (header is null)
        {
            var data = new[]
                {
                    ("Name", Metadata.ThemeName),
                    ("Version", Metadata.Version),
                    ("Author", Metadata.Author),
                    ("Maintainer", Metadata.Maintainer),
                    ("License", Metadata.License),
                    ("Description", Metadata.Description),
                    ("Homepage", Metadata.Homepage),
                    ("Repository", Metadata.Repository),
                    ("Last change", Metadata.LastUpdated?.ToString("yyyy-MM-dd dddd", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")))
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
        }


        var body = new List<string>();
        foreach (var item in items)
        {
             var formattable = item.Extract();

             if (formattable.Link is not null)
             {
                 body.Add($"hi link {formattable.Name} {formattable.Link}");
                 continue;
             }

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

             sb.Append($"hi {formattable.Name}{(formattable.Default ? " default" : "")}");
             Set("ctermfg", formattable.Style?.Foreground.TerminalColorCode);
             Set("ctermbg", formattable.Style?.Background.TerminalColorCode);
             Set("ctermul", formattable.Style?.Special.TerminalColorCode);
             Set("cterm", CreateAttrList(formattable.Style?.Modifiers));
             Set("guifg", formattable.Style?.Foreground);
             Set("guibg", formattable.Style?.Background);
             Set("guisp", formattable.Style?.Special);
             Set("gui", CreateAttrList(formattable.Style?.Modifiers));
             body.Add(sb.ToString());
        }

        var footer = Metadata.Footer(Metadata);
        if (footer is null)
        {
            footer = new[] { $"Built with Sccg {Metadata.__SccgVersion}" };
        }

        return new SingleTextContent($"colors/{Metadata.ThemeName}.vim",
            string.Join('\n', header.Select(x => $"\" {x}")),
            $"""
            hi clear
            if exists('syntax_on')
              syntax reset
            endif
            let g:colors_name = '{Metadata.ThemeName ?? "sccg_default"}'
            """,
            string.Join('\n', body),
            string.Join('\n', footer.Select(x => $"\" {x}"))
        );
    }

    public readonly record struct Formattable(
        string Name,
        Style? Style,
        string? Link,
        bool Default = false
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