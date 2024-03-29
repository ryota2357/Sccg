using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for Neovim LSP diagnostic highlight.
/// </summary>
public abstract class NeovimLspDiagnosticHighlightSource
    : Source<NeovimLspDiagnosticHighlightSource.Group, NeovimLspDiagnosticHighlightSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    /// <inheritdoc />
    public override string Name => "NeovimLspDiagnosticHighlight";

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();
        var save = new Dictionary<Group, Style>();

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
    protected override void Set(Group group, Style style) => _impl.Set(group, style);

    /// <inheritdoc />
    protected override void Link(Group from, Group to) => _impl.Link(from, to);

    /// <summary>
    /// SourceItem for Neovim LSP diagnostic highlight.
    /// </summary>
    public class Item : INeovimSourceItem
    {
        private readonly Kind _kind;

        /// <summary>
        /// Gets the group to set.
        /// </summary>
        public readonly Group Group;

        /// <summary>
        /// Gets the style to set.
        /// </summary>
        public readonly Style? Style;

        /// <summary>
        /// Gets the group to link.
        /// </summary>
        public readonly Group? Link;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(Group group, Style style)
        {
            _kind = Kind.Set;
            Group = group;
            Style = style;
            Link = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(Group from, Group to, Style? style = null)
        {
            _kind = Kind.Link;
            Group = from;
            Style = style;
            Link = to;
        }

        /// <inheritdoc />
        public NeovimFormatter.Formattable Extract()
        {
            return _kind switch
            {
                Kind.Set => new NeovimFormatter.Formattable
                {
                    Name = Group.ToString(),
                    Id = 0,
                    Style = Style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Name = Group.ToString(),
                    Id = 0,
                    Link = Link!.Value.ToString()
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

    /// <summary>
    /// :h diagnostic-highlights
    /// </summary>
    /// https://github.com/neovim/neovim/blob/master/runtime/doc/diagnostic.txt#L176
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