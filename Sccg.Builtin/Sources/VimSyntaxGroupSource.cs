using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

public abstract class VimSyntaxGroupSource : Source<VimSyntaxGroupSource.Group, VimSyntaxGroupSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    public override string Name => "VimSyntaxGroup";

    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();

        foreach (var id in ids)
        {
            var data = _impl.Store.Load(id).data;
            var next = _impl.Graph.GetLink(id);

            if (data is Group fromGroup && next is not null)
            {
                var to = _impl.Store.Load(next.Value).data;
                switch (to)
                {
                    case Group toGroup:
                        yield return new Item(fromGroup, toGroup);
                        break;
                    case Style style:
                        yield return new Item(fromGroup, style);
                        break;
                }
            }
        }
    }

    protected override void Set(Group group, Style style) => _impl.Set(group, style);

    protected override void Link(Group from, Group to) => _impl.Link(from, to);

    public sealed class Item : IVimSourceItem, INeovimSourceItem
    {
        private readonly Kind _kind;
        private readonly Group _group;
        private readonly Style? _style;
        private readonly Group? _link;

        public Item(Group group, Style style)
        {
            _kind = Kind.Set;
            _group = group;
            _style = style;
        }

        public Item(Group group, Group link)
        {
            _kind = Kind.Link;
            _group = group;
            _link = link;
        }

        VimFormatter.Formattable IVimSourceItem.Extract()
        {
            return _kind switch
            {
                Kind.Link => new VimFormatter.Formattable
                {
                    Name = _group.ToString(),
                    Link = _link.ToString()
                },
                Kind.Set => new VimFormatter.Formattable
                {
                    Name = _group.ToString(),
                    Style = _style
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
                    Name = _group.ToString(),
                    Id = 0,
                    Style = _style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Name = _group.ToString(),
                    Id = 0,
                    Link = _link!.Value.ToString()
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

    public enum Group
    {
        /// <summary>
        /// any comment
        /// </summary>
        Comment,

        /// <summary>
        /// any constant
        /// </summary>
        Constant,

        /// <summary>
        /// a string constant: "this is a string"
        /// </summary>
        String,

        /// <summary>
        /// a character constant: 'c', '\n'
        /// </summary>
        Character,

        /// <summary>
        /// a number constant: 234, 0xff
        /// </summary>
        Number,

        /// <summary>
        /// a boolean constant: TRUE, false
        /// </summary>
        Boolean,

        /// <summary>
        /// a floating point constant: 2.3e10
        /// </summary>
        Float,

        /// <summary>
        /// any variable name
        /// </summary>
        Identifier,

        /// <summary>
        /// function name (also: methods for classes)
        /// </summary>
        Function,

        /// <summary>
        /// any statement
        /// </summary>
        Statement,

        /// <summary>
        /// if, then, else, endif, switch, etc.
        /// </summary>
        Conditional,

        /// <summary>
        /// for, do, while, etc.
        /// </summary>
        Repeat,

        /// <summary>
        /// case, default, etc.
        /// </summary>
        Label,

        /// <summary>
        /// "sizeof", "+", "*", etc.
        /// </summary>
        Operator,

        /// <summary>
        /// any other keyword
        /// </summary>
        Keyword,

        /// <summary>
        /// try, catch, throw
        /// </summary>
        Exception,

        /// <summary>
        /// generic Preprocessor
        /// </summary>
        PreProc,

        /// <summary>
        /// preprocessor #include
        /// </summary>
        Include,

        /// <summary>
        /// preprocessor #define
        /// </summary>
        Define,

        /// <summary>
        /// same as Define
        /// </summary>
        Macro,

        /// <summary>
        /// preprocessor #if, #else, #endif, etc.
        /// </summary>
        PreCondit,

        /// <summary>
        /// int, long, char, etc.
        /// </summary>
        Type,

        /// <summary>
        /// static, register, volatile, etc.
        /// </summary>
        StorageClass,

        /// <summary>
        /// struct, union, enum, etc.
        /// </summary>
        Structure,

        /// <summary>
        /// A typedef
        /// </summary>
        Typedef,

        /// <summary>
        /// any special symbol
        /// </summary>
        Special,

        /// <summary>
        /// special character in a constant
        /// </summary>
        SpecialChar,

        /// <summary>
        /// you can use CTRL-] on this
        /// </summary>
        Tag,

        /// <summary>
        /// character that needs attention
        /// </summary>
        Delimiter,

        /// <summary>
        /// special things inside a comment
        /// </summary>
        SpecialComment,

        /// <summary>
        /// debugging statements
        /// </summary>
        Debug,

        /// <summary>
        /// text that stands out, HTML links
        /// </summary>
        Underlined,

        /// <summary>
        /// left blank, hidden  |hl-Ignore|
        /// </summary>
        Ignore,

        /// <summary>
        /// any erroneous construct
        /// </summary>
        Error,

        /// <summary>
        /// anything that needs extra attention; mostly the keywords TODO FIXME and XXX
        /// </summary>
        Todo
    }
}