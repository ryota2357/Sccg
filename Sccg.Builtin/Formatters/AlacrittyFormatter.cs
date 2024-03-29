using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Writers;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

/// <summary>
/// SourceItem for Alacritty config.
/// </summary>
public interface IAlacrittySourceItem : ISourceItem
{
    /// <summary>
    /// Extract <see cref="AlacrittyFormatter.Formattable"/> from this item.
    /// </summary>
    public AlacrittyFormatter.Formattable? Extract();
}

/// <summary>
/// Alacritty config with YAML. This formatter formats <see cref="IAlacrittySourceItem"/>.
/// </summary>
public class AlacrittyFormatter : Formatter<IAlacrittySourceItem, SingleTextContent>
{
    /// <inheritdoc />
    public override string Name => "Alacritty";

    /// <inheritdoc />
    protected override SingleTextContent Format(IEnumerable<IAlacrittySourceItem> items, BuilderQuery query)
    {
        var metadata = query.GetMetadata();
        var header = StdFormatterImpl.CreateHeader(metadata, "#");
        var body = CreateBody(items.Select(i => i.Extract()).WhereNotNull().ToList());
        return new SingleTextContent($"{metadata.ThemeName}.yaml", $"{string.Join('\n', header)}\n{body}");
    }

    private static string CreateBody(List<Formattable> items)
    {
        items.Sort((a, b) =>
        {
            var min = Math.Min(a.Keys.Count, b.Keys.Count);
            for (var i = 0; i < min; i++)
            {
                var cmp = string.Compare(a.Keys[i], b.Keys[i], StringComparison.Ordinal);
                if (cmp != 0)
                {
                    return cmp;
                }
            }
            return a.Keys.Count.CompareTo(b.Keys.Count);
        });
        var sb = new StringBuilder();
        for (var i = 0; i < items.Count; i++)
        {
            for (var depth = 0; depth < items[i].Keys.Count; depth++)
            {
                if (i > 0 && depth < items[i - 1].Keys.Count && items[i - 1].Keys[depth] == items[i].Keys[depth])
                {
                    continue;
                }
                sb.Append(' ', depth * 2);
                sb.Append(items[i].Keys[depth]);
                sb.Append(':');
                if (depth == items[i].Keys.Count - 1)
                {
                    sb.Append(' ');
                    sb.Append(items[i].Value);
                }
                sb.AppendLine();
            }
        }
        return sb.ToString()[..^1]; // remove last newline
    }


    /// <summary>
    /// Formattable item for Alacritty config.
    /// </summary>
    public readonly record struct Formattable(
        ReadOnlyCollection<string> Keys,
        string Value
    )
    {
        /// <inheritdoc cref="Formattable"/>
        public Formattable(IEnumerable<string> keys, string value) : this(keys.ToList().AsReadOnly(), value)
        {
        }
    }
}