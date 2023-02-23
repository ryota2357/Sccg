using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

public class NeovimLspDiagnosticHighlightSource
    : Source<NeovimLspDiagnosticHighlightSource.Group, NeovimLspDiagnosticHighlightSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    public override string Name => "NeovimLspDiagnosticHighlight";

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

    public class Item : INeovimSourceItem
    {
        private readonly Kind _kind;

        public readonly Group FromGroup;
        public readonly Style? Style;
        public readonly Group? ToGroup;

        public Item(Group group, Style style)
        {
            _kind = Kind.Set;
            FromGroup = group;
            Style = style;
            ToGroup = null;
        }

        public Item(Group from, Group to)
        {
            _kind = Kind.Link;
            FromGroup = from;
            Style = null;
            ToGroup = to;
        }

        public NeovimFormatter.Formattable Extract()
        {
            return _kind switch
            {
                Kind.Set => new NeovimFormatter.Formattable
                {
                    Name = FromGroup.ToString(),
                    Id = 0,
                    Style = Style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Name = FromGroup.ToString(),
                    Id = 0,
                    Link = ToGroup!.Value.ToString()
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
        /// Used as the base highlight group. Other Diagnostic highlights link to this by default (except Underline)
        /// </summary>
        DiagnosticError,

        /// <summary>
        /// Used as the base highlight group. Other Diagnostic highlights link to this by default (except Underline)
        /// </summary>
        DiagnosticWarn,

        /// <summary>
        /// Used as the base highlight group. Other Diagnostic highlights link to this by default (except Underline)
        /// </summary>
        DiagnosticInfo,

        /// <summary>
        /// Used as the base highlight group. Other Diagnostic highlights link to this by default (except Underline)
        /// </summary>
        DiagnosticHint,

        /// <summary>
        /// Used as the base highlight group. Other Diagnostic highlights link to this by default (except Underline)
        /// </summary>
        DiagnosticOk,

        /// <summary>
        /// Used for "Error" diagnostic virtual text.
        /// </summary>
        DiagnosticVirtualTextError,

        /// <summary>
        /// Used for "Warn" diagnostic virtual text.
        /// </summary>
        DiagnosticVirtualTextWarn,

        /// <summary>
        /// Used for "Info" diagnostic virtual text.
        /// </summary>
        DiagnosticVirtualTextInfo,

        /// <summary>
        /// Used for "Hint" diagnostic virtual text.
        /// </summary>
        DiagnosticVirtualTextHint,

        /// <summary>
        /// Used for "Ok" diagnostic virtual text.
        /// </summary>
        DiagnosticVirtualTextOk,

        /// <summary>
        /// Used to underline "Error" diagnostics.
        /// </summary>
        DiagnosticUnderlineError,

        /// <summary>
        /// Used to underline "Warn" diagnostics.
        /// </summary>
        DiagnosticUnderlineWarn,

        /// <summary>
        /// Used to underline "Info" diagnostics.
        /// </summary>
        DiagnosticUnderlineInfo,

        /// <summary>
        /// Used to underline "Hint" diagnostics.
        /// </summary>
        DiagnosticUnderlineHint,

        /// <summary>
        /// Used to underline "Ok" diagnostics.
        /// </summary>
        DiagnosticUnderlineOk,

        /// <summary>
        /// Used to color "Error" diagnostic messages in diagnostics float. See |vim.diagnostic.open_float()|
        /// </summary>
        DiagnosticFloatingError,

        /// <summary>
        /// Used to color "Warn" diagnostic messages in diagnostics float.
        /// </summary>
        DiagnosticFloatingWarn,

        /// <summary>
        /// Used to color "Info" diagnostic messages in diagnostics float.
        /// </summary>
        DiagnosticFloatingInfo,

        /// <summary>
        /// Used to color "Hint" diagnostic messages in diagnostics float.
        /// </summary>
        DiagnosticFloatingHint,

        /// <summary>
        /// Used to color "Ok" diagnostic messages in diagnostics float.
        /// </summary>
        DiagnosticFloatingOk,

        /// <summary>
        /// Used for "Error" signs in sign column.
        /// </summary>
        DiagnosticSignError,

        /// <summary>
        /// Used for "Warn" signs in sign column.
        /// </summary>
        DiagnosticSignWarn,

        /// <summary>
        /// Used for "Info" signs in sign column.
        /// </summary>
        DiagnosticSignInfo,

        /// <summary>
        /// Used for "Hint" signs in sign column.
        /// </summary>
        DiagnosticSignHint,

        /// <summary>
        /// Used for "Ok" signs in sign column.
        /// </summary>
        DiagnosticSignOk,
    }
}