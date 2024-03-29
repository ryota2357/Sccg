using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Sccg.Builtin.Writers;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

/// <summary>
/// SourceItem for iTerm2 color theme.
/// </summary>
public interface IIterm2SourceItem : ISourceItem
{
    /// <summary>
    /// Extract <see cref="Iterm2Formatter.Formattable"/> from this item.
    /// </summary>
    public Iterm2Formatter.Formattable? Extract();
}

/// <summary>
/// iTerm2 color theme with XML.
/// </summary>
public class Iterm2Formatter : Formatter<IIterm2SourceItem, SingleTextContent>
{
    /// <inheritdoc />
    public override string Name => "ITerm2";

    /// <inheritdoc />
    protected override SingleTextContent Format(IEnumerable<IIterm2SourceItem> items, BuilderQuery query)
    {
        var sb = new StringBuilder();
        foreach (var e in items.Select(x => x.Extract()).WhereNotNull())
        {
            sb.AppendLine($"""
            {"\t"}<key>{e.Key}</key>
            {"\t"}<dict>
            {"\t"}{"\t"}<key>Color Space</key>
            {"\t"}{"\t"}<string>sRGB</string>
            {"\t"}{"\t"}<key>Alpha Component</key>
            {"\t"}{"\t"}<real>1</real>
            {"\t"}{"\t"}<key>Red Component</key>
            {"\t"}{"\t"}<real>{(Convert.ToInt32(e.Color.HexCode[1..3], 16) / 255.0).ToString("F16")}</real>
            {"\t"}{"\t"}<key>Green Component</key>
            {"\t"}{"\t"}<real>{(Convert.ToInt32(e.Color.HexCode[3..5], 16) / 255.0).ToString("F16")}</real>
            {"\t"}{"\t"}<key>Blue Component</key>
            {"\t"}{"\t"}<real>{(Convert.ToInt32(e.Color.HexCode[5..7], 16) / 255.0).ToString("F16")}</real>
            {"\t"}</dict>
            """);
        }

        return new SingleTextContent($"{query.GetMetadata().ThemeName}.itermcolors",
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
            <dict>
            """,
            sb.ToString()[..^1], // remove last line break
            """
            </dict>
            </plist>
            """
        );
    }

    /// <summary>
    /// Formattable item for iTerm2 color theme.
    /// </summary>
    public readonly record struct Formattable(
        string Key,
        Color Color
    );
}