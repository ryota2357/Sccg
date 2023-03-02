using System;
using System.Collections.Generic;
using System.IO;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for 16 ANSI colors.
/// </summary>
public abstract class Ansi16ColorSource : SourceColorOnly<Ansi16ColorSource.Group, Ansi16ColorSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    /// <inheritdoc />
    public override string Name => "Ansi16Color";

    /// <inheritdoc />
    public override int Priority => 0;

    /// <summary>
    /// Target of this source.
    /// </summary>
    protected virtual Target ItemTarget => Target.All;

    /// <inheritdoc />
    protected override void Set(Group group, Color color) => _impl.Set(group, color);

    /// <inheritdoc />
    protected override void Link(Group from, Group to) => _impl.Link(from, to);

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();
        Dictionary<int, Color> cache = new();

        foreach (var id in ids)
        {
            // skip until group
            if (!_impl.Store.TryLoad(id, out Group group)) continue;

            // group must have link, because it is set/linked by Set/Link method.
            if (!_impl.Graph.TryGetLink(id, out var next))
            {
                throw new Exception($"Source item data is broken. Group {group} does not have link.");
            }

            if (_impl.Store.TryLoad(next.Value, out Color col) || cache.TryGetValue(next.Value, out col))
            {
                cache[id] = col;
                yield return new Item(group, col, ItemTarget);
            }
            else
            {
                // A links to B, but B is not Color or not a group that has Color.
                throw new InvalidDataException($"Group {group} does not have specific color.");
            }
        }
    }

    /// <summary>
    /// SourceItem for 16 ANSI colors.
    /// </summary>
    public sealed class Item : IVimArrayVariableSourceItem, INeovimVariableSourceItem, IIterm2SourceItem,
        IAlacrittySourceItem
    {

        /// <summary>
        /// Gets the group.
        /// </summary>
        public readonly Group Group;

        /// <summary>
        /// Gets the color.
        /// </summary>
        public readonly Color Color;

        private Target _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(Group group, Color color, Target target)
        {
            Group = group;
            Color = color;
            _target = target;
        }

        /// <summary>
        /// Tests this item is specified ANSI code color.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool IsAnsi(int code)
        {
            if ((uint)code > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(code), code, "Code must be 0-15.");
            }
            return code == (int)Group;
        }

        VimFormatter.FormattableArrayVariable? IVimArrayVariableSourceItem.Extract()
        {
            if (!_target.HasFlag(Target.Vim))
            {
                return null;
            }
            return new VimFormatter.FormattableArrayVariable(
                Name: "terminal_ansi_colors",
                Value: Color.HexCode,
                Index: (int)Group,
                Length: 16
            );
        }

        NeovimFormatter.FormattableVariable? INeovimVariableSourceItem.Extract()
        {
            if (!_target.HasFlag(Target.Neovim))
            {
                return null;
            }
            return new NeovimFormatter.FormattableVariable(
                Name: $"terminal_ansi_colors_{(int)Group}",
                Value: Color.HexCode
            );
        }

        Iterm2Formatter.Formattable? IIterm2SourceItem.Extract()
        {
            if (!_target.HasFlag(Target.Iterm2))
            {
                return null;
            }
            return new Iterm2Formatter.Formattable(
                Key: Iterm2ColorsSource.CreateKey(Group switch
                {
                    Group.Ansi0 => Iterm2ColorsSource.Group.Ansi0,
                    Group.Ansi1 => Iterm2ColorsSource.Group.Ansi1,
                    Group.Ansi2 => Iterm2ColorsSource.Group.Ansi2,
                    Group.Ansi3 => Iterm2ColorsSource.Group.Ansi3,
                    Group.Ansi4 => Iterm2ColorsSource.Group.Ansi4,
                    Group.Ansi5 => Iterm2ColorsSource.Group.Ansi5,
                    Group.Ansi6 => Iterm2ColorsSource.Group.Ansi6,
                    Group.Ansi7 => Iterm2ColorsSource.Group.Ansi7,
                    Group.Ansi8 => Iterm2ColorsSource.Group.Ansi8,
                    Group.Ansi9 => Iterm2ColorsSource.Group.Ansi9,
                    Group.Ansi10 => Iterm2ColorsSource.Group.Ansi10,
                    Group.Ansi11 => Iterm2ColorsSource.Group.Ansi11,
                    Group.Ansi12 => Iterm2ColorsSource.Group.Ansi12,
                    Group.Ansi13 => Iterm2ColorsSource.Group.Ansi13,
                    Group.Ansi14 => Iterm2ColorsSource.Group.Ansi14,
                    Group.Ansi15 => Iterm2ColorsSource.Group.Ansi15,
                    _ => throw new ArgumentOutOfRangeException()
                }),
                Color: Color
            );
        }

        AlacrittyFormatter.Formattable? IAlacrittySourceItem.Extract()
        {
            if (!_target.HasFlag(Target.Alacritty))
            {
                return null;
            }
            return new AlacrittyFormatter.Formattable(
                keys: AlacrittyColorsSource.CreateKeys(Group switch
                {
                    Group.Ansi0 => AlacrittyColorsSource.Group.NormalBlack,
                    Group.Ansi1 => AlacrittyColorsSource.Group.NormalRed,
                    Group.Ansi2 => AlacrittyColorsSource.Group.NormalGreen,
                    Group.Ansi3 => AlacrittyColorsSource.Group.NormalYellow,
                    Group.Ansi4 => AlacrittyColorsSource.Group.NormalBlue,
                    Group.Ansi5 => AlacrittyColorsSource.Group.NormalMagenta,
                    Group.Ansi6 => AlacrittyColorsSource.Group.NormalCyan,
                    Group.Ansi7 => AlacrittyColorsSource.Group.NormalWhite,
                    Group.Ansi8 => AlacrittyColorsSource.Group.BrightBlack,
                    Group.Ansi9 => AlacrittyColorsSource.Group.BrightRed,
                    Group.Ansi10 => AlacrittyColorsSource.Group.BrightGreen,
                    Group.Ansi11 => AlacrittyColorsSource.Group.BrightYellow,
                    Group.Ansi12 => AlacrittyColorsSource.Group.BrightBlue,
                    Group.Ansi13 => AlacrittyColorsSource.Group.BrightMagenta,
                    Group.Ansi14 => AlacrittyColorsSource.Group.BrightCyan,
                    Group.Ansi15 => AlacrittyColorsSource.Group.BrightWhite,
                    _ => throw new ArgumentOutOfRangeException()
                }),
                value: AlacrittyColorsSource.CreateValue(Color)
            );
        }
    }

    /// <summary>
    /// Color group (ANSI)
    /// </summary>
    public enum Group
    {
        /// <summary>
        /// Black (Normal)
        /// </summary>
        Ansi0 = 0,

        /// <summary>
        /// Red (Normal)
        /// </summary>
        Ansi1 = 1,

        /// <summary>
        /// Green (Normal)
        /// </summary>
        Ansi2 = 2,

        /// <summary>
        /// Yellow (Normal)
        /// </summary>
        Ansi3 = 3,

        /// <summary>
        /// Blue (Normal)
        /// </summary>
        Ansi4 = 4,

        /// <summary>
        /// Magenta (Normal)
        /// </summary>
        Ansi5 = 5,

        /// <summary>
        /// Cyan (Normal)
        /// </summary>
        Ansi6 = 6,

        /// <summary>
        /// White (Normal)
        /// </summary>
        Ansi7 = 7,

        /// <summary>
        /// Black (Bright)
        /// </summary>
        Ansi8 = 8,

        /// <summary>
        /// Red (Bright)
        /// </summary>
        Ansi9 = 9,

        /// <summary>
        /// Green (Bright)
        /// </summary>
        Ansi10 = 10,

        /// <summary>
        /// Yellow (Bright)
        /// </summary>
        Ansi11 = 11,

        /// <summary>
        /// Blue (Bright)
        /// </summary>
        Ansi12 = 12,

        /// <summary>
        /// Magenta (Bright)
        /// </summary>
        Ansi13 = 13,

        /// <summary>
        /// Cyan (Bright)
        /// </summary>
        Ansi14 = 14,

        /// <summary>
        /// White (Bright)
        /// </summary>
        Ansi15 = 15,
    }

    /// <summary>
    /// Selection of <see cref="Ansi16ColorSource"/>'s target.
    /// </summary>
    [Flags]
    public enum Target
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Generate <see cref="IVimArrayVariableSourceItem"/> source item.
        /// </summary>
        Vim = 1 << 0,

        /// <summary>
        /// Generate <see cref="INeovimVariableSourceItem"/> source item.
        /// </summary>
        Neovim = 1 << 1,

        /// <summary>
        /// Generate <see cref="IIterm2SourceItem"/> source item.
        /// </summary>
        Iterm2 = 1 << 2,

        /// <summary>
        /// Generate <see cref="IAlacrittySourceItem"/> source item.
        /// </summary>
        Alacritty = 1 << 3,

        /// <summary>
        /// All.
        /// </summary>
        All = Vim | Neovim | Iterm2 | Alacritty,
    }
}