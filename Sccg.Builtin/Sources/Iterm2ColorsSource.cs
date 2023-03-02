using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

// ITermColorsSource looks like interface...

public abstract class Iterm2ColorsSource : SourceColorOnly<Iterm2ColorsSource.Group, Iterm2ColorsSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    public override string Name => "ITerm2Colors";

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

    public class Item : IIterm2SourceItem
    {
        public readonly Group Group;
        public readonly Color Color;

        public Item(Group group, Color color)
        {
            Group = group;
            Color = color;
        }

        public Iterm2Formatter.Formattable Extract()
        {
            var key = Group.ToString()
                            .Aggregate(new List<StringBuilder>() { new() }, (list, next) =>
                            {
                                if (list[^1].Length == 0)
                                {
                                    list[^1].Append(next);
                                }
                                else if (char.IsLower(list[^1][^1]) && (char.IsUpper(next) || char.IsNumber(next)))
                                {
                                    list.Add(new StringBuilder(next.ToString()));
                                }
                                else
                                {
                                    list[^1].Append(next);
                                }

                                return list;
                            })
                            .Aggregate(new StringBuilder(), (builder, next) => builder.Append(next).Append(' '))
                            .Append("Color")
                            .ToString();
            return new Iterm2Formatter.Formattable
            {
                Key = key,
                Color = Color
            };
        }
    }

    public enum Group
    {
        /// <summary>
        /// Foreground color (Basic Colors)
        /// </summary>
        Foreground,

        /// <summary>
        /// Background color (Basic Colors)
        /// </summary>
        Background,

        /// <summary>
        /// Bold color (Basic Colors)
        /// </summary>
        Bold,

        /// <summary>
        /// Link color (Basic Colors)
        /// </summary>
        Link,

        /// <summary>
        /// Selection color (Basic Colors)
        /// </summary>
        Selection,

        /// <summary>
        /// Selected text color (Basic Colors)
        /// </summary>
        SelectedText,

        /// <summary>
        /// Badge color (Basic Colors)
        /// </summary>
        Badge,

        /// <summary>
        /// Tab color (Basic Colors)
        /// </summary>
        Tab,

        /// <summary>
        /// Underline color (Basic Colors)
        /// </summary>
        Underline,

        /// <summary>
        /// Cursor color (Cursor Colors)
        /// </summary>
        Cursor,

        /// <summary>
        /// Cursor text color (Cursor Colors)
        /// </summary>
        CursorText,

        /// <summary>
        /// Cursor guide color (Cursor Colors)
        /// </summary>
        CursorGuide,

        /// <summary>
        /// Black (Normal)
        /// </summary>
        Ansi0,

        /// <summary>
        /// Red (Normal)
        /// </summary>
        Ansi1,

        /// <summary>
        /// Green (Normal)
        /// </summary>
        Ansi2,

        /// <summary>
        /// Yellow (Normal)
        /// </summary>
        Ansi3,

        /// <summary>
        /// Blue (Normal)
        /// </summary>
        Ansi4,

        /// <summary>
        /// Magenta (Normal)
        /// </summary>
        Ansi5,

        /// <summary>
        /// Cyan (Normal)
        /// </summary>
        Ansi6,

        /// <summary>
        /// White (Normal)
        /// </summary>
        Ansi7,

        /// <summary>
        /// Black (Bright)
        /// </summary>
        Ansi8,

        /// <summary>
        /// Red (Bright)
        /// </summary>
        Ansi9,

        /// <summary>
        /// Green (Bright)
        /// </summary>
        Ansi10,

        /// <summary>
        /// Yellow (Bright)
        /// </summary>
        Ansi11,

        /// <summary>
        /// Blue (Bright)
        /// </summary>
        Ansi12,

        /// <summary>
        /// Magenta (Bright)
        /// </summary>
        Ansi13,

        /// <summary>
        /// Cyan (Bright)
        /// </summary>
        Ansi14,

        /// <summary>
        /// White (Bright)
        /// </summary>
        Ansi15
    }
}