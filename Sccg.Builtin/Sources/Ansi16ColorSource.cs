using System;
using System.Collections.Generic;
using System.IO;
using Sccg.Builtin.Develop;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

public abstract class Ansi16ColorSource : ISource
{
    private readonly StdSourceImpl<Group> _impl = new();

    // cache
    private bool _isCustomCalled = false;
    private IEnumerable<Item>? _items = null;

    public string Name => "Ansi16Color";

    public int Priority => 0;

    void ISource.Custom(BuilderQuery query)
    {
        if (_isCustomCalled) return;
        Custom(query);
        _isCustomCalled = true;
    }

    IEnumerable<ISourceItem> ISource.CollectItems()
    {
        return _items ??= CollectItems();
    }

    protected virtual void Custom(BuilderQuery query)
    {
        Custom();
    }

    protected virtual void Custom()
    {
        throw new NotImplementedException("You must override Custom method.");
    }

    protected void Set(Group group, Color color)
    {
        _impl.Set(group, color);
    }

    protected void Link(Group from, Group to)
    {
        _impl.Link(from, to);
    }

    private IEnumerable<Item> CollectItems()
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
                yield return new Item(group, col);
            }
            else
            {
                // A links to B, but B is not Color or not a group that has Color.
                throw new InvalidDataException($"Group {group} does not have specific color.");
            }
        }
    }

    public sealed class Item : ISourceItem
    {
        public readonly Group Group;
        public readonly Color Color;

        public Item(Group group, Color color)
        {
            Group = group;
            Color = color;
        }

        public bool IsAnsi(int code)
        {
            if ((uint)code > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(code), code, "Code must be 0-15.");
            }
            return code == (int)Group;
        }
    }

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
}