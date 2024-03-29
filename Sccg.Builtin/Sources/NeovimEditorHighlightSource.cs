using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for Neovim's editor highlight-groups.
/// </summary>
public abstract class NeovimEditorHighlightSource
    : Source<NeovimEditorHighlightSource.Group, NeovimEditorHighlightSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    /// <inheritdoc />
    public override string Name => "NeovimEditorHighlight";

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
    /// SourceItem for Neovim's editor highlight-groups.
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
    /// h: highlight-groups
    /// </summary>
    /// https://github.com/neovim/neovim/blob/master/runtime/doc/syntax.txt#L5154
    public enum Group
    {
        /// <summary>
        /// Used for the columns set with 'colorcolumn'.
        /// </summary>
        ColorColumn,

        /// <summary>
        /// Placeholder characters substituted for concealed text (see 'conceallevel').
        /// </summary>
        Conceal,

        /// <summary>
        /// Used for highlighting a search pattern under the cursor (see 'hlsearch').
        /// </summary>
        CurSearch,

        /// <summary>
        /// Character under the cursor.
        /// </summary>
        Cursor,

        /// <summary>
        /// Character under the cursor when |language-mapping| is used (see 'guicursor').
        /// </summary>
        lCursor,

        /// <summary>
        /// Like Cursor, but used when in IME mode. *CursorIM*
        /// </summary>
        CursorIM,

        /// <summary>
        /// Screen-column at the cursor, when 'cursorcolumn' is set.
        /// </summary>
        CursorColumn,

        /// <summary>
        /// Screen-line at the cursor, when 'cursorline' is set. Low-priority if foreground (ctermfg OR guifg) is not set.
        /// </summary>
        CursorLine,

        /// <summary>
        /// Directory names (and other special names in listings).
        /// </summary>
        Directory,

        /// <summary>
        /// Diff mode: Added line. |diff.txt|
        /// </summary>
        DiffAdd,

        /// <summary>
        /// Diff mode: Changed line. |diff.txt|
        /// </summary>
        DiffChange,

        /// <summary>
        /// Diff mode: Deleted line. |diff.txt|
        /// </summary>
        DiffDelete,

        /// <summary>
        /// Diff mode: Changed text within a changed line. |diff.txt|
        /// </summary>
        DiffText,

        /// <summary>
        /// Filler lines (~) after the end of the buffer. By default, this is highlighted like |hl-NonText|.
        /// </summary>
        EndOfBuffer,

        /// <summary>
        /// Cursor in a focused terminal.
        /// </summary>
        TermCursor,

        /// <summary>
        /// Cursor in an unfocused terminal.
        /// </summary>
        TermCursorNC,

        /// <summary>
        /// Error messages on the command line.
        /// </summary>
        ErrorMsg,

        /// <summary>
        /// Separators between window splits.
        /// </summary>
        WinSeparator,

        /// <summary>
        /// Line used for closed folds.
        /// </summary>
        Folded,

        /// <summary>
        /// 'foldcolumn'
        /// </summary>
        FoldColumn,

        /// <summary>
        /// Column where |signs| are displayed.
        /// </summary>
        SignColumn,

        /// <summary>
        /// 'incsearch' highlighting; also used for the text replaced with ":s///c".
        /// </summary>
        IncSearch,

        /// <summary>
        /// |:substitute| replacement text highlighting.
        /// </summary>
        Substitute,

        /// <summary>
        /// Line number for ":number" and ":#" commands, and when 'number' or 'relativenumber' option is set.
        /// </summary>
        LineNr,

        /// <summary>
        /// Line number for when the 'relativenumber' option is set, above the cursor line.
        /// </summary>
        LineNrAbove,

        /// <summary>
        /// Line number for when the 'relativenumber' option is set, below the cursor line.
        /// </summary>
        LineNrBelow,

        /// <summary>
        /// Like LineNr when 'cursorline' is set and 'cursorlineopt' contains "number" or is "both", for the cursor line.
        /// </summary>
        CursorLineNr,

        /// <summary>
        /// Like FoldColumn when 'cursorline' is set for the cursor line.
        /// </summary>
        CursorLineFold,

        /// <summary>
        /// Like SignColumn when 'cursorline' is set for the cursor line.
        /// </summary>
        CursorLineSign,

        /// <summary>
        /// Character under the cursor or just before it, if it is a paired bracket, and its match. |pi_paren.txt|
        /// </summary>
        MatchParen,

        /// <summary>
        /// 'showmode' message (e.g., "-- INSERT --").
        /// </summary>
        ModeMsg,

        /// <summary>
        /// Area for messages and cmdline.
        /// </summary>
        MsgArea,

        /// <summary>
        /// Separator for scrolled messages |msgsep|.
        /// </summary>
        MsgSeparator,

        /// <summary>
        /// |more-prompt|
        /// </summary>
        MoreMsg,

        /// <summary>
        /// '@' at the end of the window, characters from 'showbreak' and other characters that do not really exist in the text (e.g., ">" displayed when a double-wide character doesn't fit at the end of the line). See also |hl-EndOfBuffer|.
        /// </summary>
        NonText,

        /// <summary>
        /// Normal text.
        /// </summary>
        Normal,

        /// <summary>
        /// Normal text in floating windows.
        /// </summary>
        NormalFloat,

        /// <summary>
        /// Floating window border.
        /// </summary>
        /// <remarks>This group is not documented in the help file.</remarks>
        FloatBorder,

        /// <summary>
        /// Normal text in non-current windows.
        /// </summary>
        NormalNC,

        /// <summary>
        /// Popup menu: Normal item.
        /// </summary>
        Pmenu,

        /// <summary>
        /// Popup menu: Selected item.
        /// </summary>
        PmenuSel,

        /// <summary>
        /// Popup menu: Scrollbar.
        /// </summary>
        PmenuSbar,

        /// <summary>
        /// Popup menu: Thumb of the scrollbar.
        /// </summary>
        PmenuThumb,

        /// <summary>
        /// |hit-enter| prompt and yes/no questions.
        /// </summary>
        Question,

        /// <summary>
        /// Current |quickfix| item in the quickfix window. Combined with  |hl-CursorLine| when the cursor is there.
        /// </summary>
        QuickFixLine,

        /// <summary>
        /// Last search pattern highlighting (see 'hlsearch'). Also used for similar items that need to stand out.
        /// </summary>
        Search,

        /// <summary>
        /// Unprintable characters: Text displayed differently from what it really is. But not 'listchars' whitespace. |hl-Whitespace|
        /// </summary>
        SpecialKey,

        /// <summary>
        /// Word that is not recognized by the spellchecker. |spell| Combined with the highlighting used otherwise.
        /// </summary>
        SpellBad,

        /// <summary>
        /// Word that should start with a capital. |spell| Combined with the highlighting used otherwise.
        /// </summary>
        SpellCap,

        /// <summary>
        /// Word that is recognized by the spellchecker as one that is used in another region. |spell| Combined with the highlighting used otherwise.
        /// </summary>
        SpellLocal,

        /// <summary>
        /// Word that is recognized by the spellchecker as one that is hardly ever used. |spell| Combined with the highlighting used otherwise.
        /// </summary>
        SpellRare,

        /// <summary>
        /// Status line of current window.
        /// </summary>
        StatusLine,

        /// <summary>
        /// Status lines of not-current windows. Note: If this is equal to "StatusLine", Vim will use "^^^" in the status line of the current window.
        /// </summary>
        StatusLineNC,

        /// <summary>
        /// Tab pages line, not active tab page label.
        /// </summary>
        TabLine,

        /// <summary>
        /// Tab pages line, where there are no labels.
        /// </summary>
        TabLineFill,

        /// <summary>
        /// Tab pages line, active tab page label.
        /// </summary>
        TabLineSel,

        /// <summary>
        /// Titles for output from ":set all", ":autocmd" etc.
        /// </summary>
        Title,

        /// <summary>
        /// Visual mode selection.
        /// </summary>
        Visual,

        /// <summary>
        /// Visual mode selection when vim is "Not Owning the Selection".
        /// </summary>
        VisualNOS,

        /// <summary>
        /// Warning messages.
        /// </summary>
        WarningMsg,

        /// <summary>
        /// "nbsp", "space", "tab", "multispace", "lead" and "trail" in 'listchars'.
        /// </summary>
        Whitespace,

        /// <summary>
        /// Current match in 'wildmenu' completion.
        /// </summary>
        WildMenu,

        /// <summary>
        /// Window bar of current window.
        /// </summary>
        WinBar,

        /// <summary>
        /// Window bar of not-current windows.
        /// </summary>
        WinBarNC,

        /// <summary>
        /// Current font, background and foreground colors of the menus. Also used for the toolbar. Applicable highlight arguments: font, guibg, guifg.
        /// </summary>
        Menu,

        /// <summary>
        /// Current background and foreground of the main window's scrollbars. Applicable highlight arguments: guibg, guifg.
        /// </summary>
        Scrollbar,

        /// <summary>
        /// Current font, background and foreground of the tooltips. Applicable highlight arguments: font, guibg, guifg.
        /// </summary>
        Tooltip
    }
}