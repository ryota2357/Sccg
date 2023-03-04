using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for Vim/Neovim custom highlight group.
/// </summary>
public abstract class VimCustomGroupSource : Source<string, VimCustomGroupSource.Item>
{
    /// <inheritdoc />
    public override abstract string Name { get; }

    private readonly StdSourceImpl<string> _impl = new();

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();
        var save = new Dictionary<string, Style>();

        foreach (var id in ids)
        {
            var data = _impl.Store.Load(id).data;
            var next = _impl.Graph.GetLink(id);

            if (data is string fromGroup && next is not null)
            {
                var to = _impl.Store.Load(next.Value).data;
                switch (to)
                {
                    case string toGroup:
                        Style? sty = save.ContainsKey(toGroup) ? save[toGroup] : null;
                        if (sty is not null)
                        {
                            save[fromGroup] = sty.Value;
                        }
                        yield return new Item(fromGroup, toGroup, sty);
                        break;
                    case Style style:
                        save[fromGroup] = style;
                        yield return new Item(fromGroup, style);
                        break;
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void Set(string group, Style style) => _impl.Set(group, style);

    /// <inheritdoc />
    protected override void Link(string from, string to) => _impl.Link(from, to);

    /// <summary>
    /// SourceItem for Vim/Neovim custom highlight group.
    /// </summary>
    public class Item : IVimSourceItem, INeovimSourceItem
    {
        private readonly Kind _kind;

        /// <summary>
        /// Gets the group to set.
        /// </summary>
        public readonly string Group;

        /// <summary>
        /// Gets the style to set.
        /// </summary>
        public readonly Style? Style;

        /// <summary>
        /// Gets the group to link.
        /// </summary>
        public readonly string? Link;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(string group, Style style)
        {
            _kind = Kind.Set;
            Group = group;
            Style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(string group, string link, Style? style = null)
        {
            _kind = Kind.Link;
            Group = group;
            Link = link;
            Style = style;
        }

        VimFormatter.Formattable IVimSourceItem.Extract()
        {
            return _kind switch
            {
                Kind.Link => new VimFormatter.Formattable
                {
                    Name = Group,
                    Link = Link
                },
                Kind.Set => new VimFormatter.Formattable
                {
                    Name = Group,
                    Style = Style
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        NeovimFormatter.Formattable INeovimSourceItem.Extract()
        {
            return _kind switch
            {
                Kind.Set => new NeovimFormatter.Formattable
                {
                    Name = Group,
                    Id = 0,
                    Style = Style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Name = Group,
                    Id = 0,
                    Link = Link
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum Kind
        {
            Set,
            Link
        }
    }
}