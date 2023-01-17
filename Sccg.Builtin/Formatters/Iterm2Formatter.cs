using System;
using System.Text;
using System.Collections.Generic;
using Sccg.Core;
using Sccg.Builtin.ContentWriters;

namespace Sccg.Builtin.Formatters;

public interface IIterm2SourceItem : ISourceItem
{
    public Iterm2Formatter.Formattable Extract();
}

public class Iterm2Formatter : Formatter<IIterm2SourceItem, SingleTextContent>, IMetadataUser
{
    public Metadata Metadata { get; set; } = Metadata.Empty;

    public override string Name => "ITerm2";

    protected override SingleTextContent Format(IEnumerable<IIterm2SourceItem> items)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            var e = item.Extract();
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

        return new SingleTextContent($"{Metadata.ThemeName}.itermcolors",
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
            <dict>
            """,
            sb.ToString()[..^1], // remove last newline
            """
            </dict>
            </plist>
            """
        );
    }

    public readonly record struct Formattable(
        string Key,
        Color Color
    );
}