using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for Alacritty colors.
/// </summary>
public abstract partial class AlacrittyColorsSource : SourceColorOnly<AlacrittyColorsSource.Group, AlacrittyColorsSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    /// <inheritdoc />
    public override string Name => "AlacrittyColors";

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();

        Dictionary<int, Color> cache = new();
        foreach (var id in ids)
        {
            if (!_impl.Store.TryLoad(id, out Group group)) continue;
            if (!_impl.Graph.TryGetLink(id, out var next)) continue;

            if (cache.TryGetValue(next.Value, out var col) || _impl.Store.TryLoad(next.Value, out col))
            {
                cache[id] = col;
                yield return new Item(group, col);
            }
            else
            {
                throw new InvalidDataException($"Group {group} does not have specific color.");
            }
        }
    }

    /// <inheritdoc />
    protected override void Set(Group group, Color color) => _impl.Set(group, color);

    /// <inheritdoc />
    protected override void Link(Group from, Group to) => _impl.Link(from, to);

    /// <summary>
    /// SourceItem for Alacritty.
    /// </summary>
    public partial class Item : IAlacrittySourceItem
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        public readonly Group Group;

        /// <summary>
        /// Gets the color.
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(Group group, Color color)
        {
            Group = group;
            Color = color;
        }

        /// <inheritdoc />
        public AlacrittyFormatter.Formattable? Extract()
        {
            return new AlacrittyFormatter.Formattable(
                Keys: CreateKeys(Group),
                Value: CreateValue(Color)
            );
        }

    }

    internal static ReadOnlyCollection<string> CreateKeys(Group group)
    {
        return LargeChar().Replace(group.ToString(), ".$1")
                          .Split('.')
                          .Skip(1)
                          .Select(s => s.ToLower())
                          .Prepend("colors")
                          .ToArray()
                          .AsReadOnly();
    }

    internal static string CreateValue(Color color)
    {
        if (color.IsDefault)
        {
            throw new NotSupportedException("Alacritty does not support Color.Default.");
        }
        return color.IsNone ? "None" : $"'{color.HexCode}'";
    }

    [GeneratedRegex("([A-Z])")]
    private static partial Regex LargeChar();

    /// <summary>
    /// Color group for Alacritty.
    /// </summary>
    public enum Group
    {
        /// <summary>
        /// color.primary.background
        /// </summary>
        PrimaryBackground,

        /// <summary>
        /// color.primary.foreground
        /// </summary>
        PrimaryForeground,

        /// <summary>
        /// color.primary.dim_foreground
        /// </summary>
        /// <remarks>The dimmed foreground color is calculated automatically if it is not present.</remarks>
        PrimaryDim_foreground,

        /// <summary>
        /// color.primary.bright_foreground
        /// </summary>
        /// <remarks>If the bright foreground color is not set, or `draw_bold_text_with_bright_colors` is `false`, the normal foreground color will be used.</remarks>
        PrimaryBright_foreground,

        /// <summary>
        /// color.normal.black
        /// </summary>
        NormalBlack,

        /// <summary>
        /// color.normal.red
        /// </summary>
        NormalRed,

        /// <summary>
        /// color.normal.green
        /// </summary>
        NormalGreen,

        /// <summary>
        /// color.normal.yellow
        /// </summary>
        NormalYellow,

        /// <summary>
        /// color.normal.blue
        /// </summary>
        NormalBlue,

        /// <summary>
        /// color.normal.magenta
        /// </summary>
        NormalMagenta,

        /// <summary>
        /// color.normal.cyan
        /// </summary>
        NormalCyan,

        /// <summary>
        /// color.normal.white
        /// </summary>
        NormalWhite,

        /// <summary>
        /// color.bright.black
        /// </summary>
        BrightBlack,

        /// <summary>
        /// color.bright.red
        /// </summary>
        BrightRed,

        /// <summary>
        /// color.bright.green
        /// </summary>
        BrightGreen,

        /// <summary>
        /// color.bright.yellow
        /// </summary>
        BrightYellow,

        /// <summary>
        /// color.bright.blue
        /// </summary>
        BrightBlue,

        /// <summary>
        /// color.bright.magenta
        /// </summary>
        BrightMagenta,

        /// <summary>
        /// color.bright.cyan
        /// </summary>
        BrightCyan,

        /// <summary>
        /// color.bright.white
        /// </summary>
        BrightWhite,

        /// <summary>
        /// color.dim.black
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimBlack,

        /// <summary>
        /// color.dim.red
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimRed,

        /// <summary>
        /// color.dim.green
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimGreen,

        /// <summary>
        /// color.dim.yellow
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimYellow,

        /// <summary>
        /// color.dim.blue
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimBlue,

        /// <summary>
        /// color.dim.magenta
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimMagenta,

        /// <summary>
        /// color.dim.cyan
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimCyan,

        /// <summary>
        /// color.dim.white
        /// </summary>
        /// <remarks>If the dim colors are not set, they will be calculated automatically based on the `normal` colors.</remarks>
        DimWhite,
    }
}