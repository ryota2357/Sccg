using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

public abstract class VimEditorHighlightSource : Source<VimEditorHighlightSource.Group, VimEditorHighlightSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    public override string Name => "VimEditorHighlight";

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

    public sealed class Item : IVimSourceItem
    {
        private readonly Kind _kind;

        public readonly Group Group;
        public readonly Style? Style;
        public readonly Group? Link;

        public Item(Group group, Style style)
        {
            _kind = Kind.Set;
            Group = group;
            Style = style;
        }

        public Item(Group group, Group link, Style? style = null)
        {
            _kind = Kind.Link;
            Group = group;
            Link = link;
            Style = style;
        }

        public VimFormatter.Formattable Extract()
        {
            return _kind switch
            {
                Kind.Link => new VimFormatter.Formattable
                {
                    Name = Group.ToString(),
                    Link = Link.ToString()
                },
                Kind.Set => new VimFormatter.Formattable
                {
                    Name = Group.ToString(),
                    Style = Style
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
    /// :h highlight-groups
    /// </summary>
    /// https://github.com/vim/vim/blob/master/runtime/doc/syntax.txt#L5346
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
        /// Character under the cursor.
        /// </summary>
        Cursor,

        /// <summary>
        /// Character under the cursor when |language-mapping| is used (see 'guicursor').
        /// </summary>
        lCursor,

        /// <summary>
        /// Like Cursor, but used when in IME mode. |CursorIM|
        /// </summary>
        CursorIM,

        /// <summary>
        /// Screen column that the cursor is in when 'cursorcolumn' is set.
        /// </summary>
        CursorColumn,

        /// <summary>
        /// Screen line that the cursor is in when 'cursorline' is set.
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
        /// Filler lines (~) after the last line in the buffer. By default, this is highlighted like |hl-NonText|.
        /// </summary>
        EndOfBuffer,

        /// <summary>
        /// Error messages on the command line.
        /// </summary>
        ErrorMsg,

        /// <summary>
        /// Column separating vertically split windows.
        /// </summary>
        VertSplit,

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
        /// Messages popup window used by `:echowindow`. If not defined |hl-WarningMsg| is used.
        /// </summary>
        MessageWindow,

        /// <summary>
        /// 'showmode' message (e.g., "-- INSERT --").
        /// </summary>
        ModeMsg,

        /// <summary>
        /// |more-prompt|
        /// </summary>
        MoreMsg,

        /// <summary>
        /// '@' at the end of the window, "<<<" at the start of the window for 'smoothscroll', characters from 'showbreak' and other characters that do not really exist in the text, such as the ">" displayed when a double-wide character doesn't fit at the end of the line.
        /// </summary>
        NonText,

        /// <summary>
        /// Normal text.
        /// </summary>
        Normal,

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
        /// Popup window created with |popup_notification()|.If not defined |hl-WarningMsg| is used.
        /// </summary>
        PopupNotification,

        /// <summary>
        /// |hit-enter| prompt and yes/no questions.
        /// </summary>
        Question,

        /// <summary>
        /// Current |quickfix| item in the quickfix window.
        /// </summary>
        QuickFixLine,

        /// <summary>
        /// Last search pattern highlighting (see 'hlsearch'). Also used for similar items that need to stand out.
        /// </summary>
        Search,

        /// <summary>
        /// Current match for the last search pattern (see 'hlsearch'). Note: This is correct after a search, but may get outdated if changes are made or the screen is redrawn.
        /// </summary>
        CurSearch,

        /// <summary>
        /// Meta and special keys listed with ":map", also for text used to show unprintable characters in the text, 'listchars'. Generally: Text that is displayed differently from what it really is.
        /// </summary>
        SpecialKey,

        /// <summary>
        /// Word that is not recognized by the spellchecker. |spell| This will be combined with the highlighting used otherwise.
        /// </summary>
        SpellBad,

        /// <summary>
        /// Word that should start with a capital. |spell| This will be combined with the highlighting used otherwise.
        /// </summary>
        SpellCap,

        /// <summary>
        /// Word that is recognized by the spellchecker as one that is used in another region. |spell| This will be combined with the highlighting used otherwise.
        /// </summary>
        SpellLocal,

        /// <summary>
        /// Word that is recognized by the spellchecker as one that is hardly ever used. |spell| This will be combined with the highlighting used otherwise.
        /// </summary>
        SpellRare,

        /// <summary>
        /// Status line of current window.
        /// </summary>
        StatusLine,

        /// <summary>
        /// status lines of not-current windows Note: If this is equal to "StatusLine", Vim will use "^^^" in the status line of the current window.
        /// </summary>
        StatusLineNC,

        /// <summary>
        /// Status line of current window, if it is a |terminal| window.
        /// </summary>
        StatusLineTerm,

        /// <summary>
        /// Status lines of not-current windows that is a |terminal| window.
        /// </summary>
        StatusLineTermNC,

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
        /// |terminal| window (see |terminal-size-color|).
        /// </summary>
        Terminal,

        /// <summary>
        /// Titles for output from ":set all", ":autocmd" etc.
        /// </summary>
        Title,

        /// <summary>
        /// Visual mode selection.
        /// </summary>
        Visual,

        /// <summary>
        /// Visual mode selection when vim is "Not Owning the Selection". Only X11 Gui's |gui-x11| and |xterm-clipboard| supports this.
        /// </summary>
        VisualNOS,

        /// <summary>
        /// Warning messages.
        /// </summary>
        WarningMsg,

        /// <summary>
        /// Current match in 'wildmenu' completion.
        /// </summary>
        WildMenu,

        /// <summary>
        /// Current font, background and foreground colors of the menus. Also used for the toolbar. Applicable highlight arguments: font, guibg, guifg. NOTE: For Motif the font argument actually specifies a fontset at all times, no matter if 'guifontset' is empty, and as such it is tied to the current |:language| when set.
        /// </summary>
        Menu,

        /// <summary>
        /// Current background and foreground of the main window's scrollbars. Applicable highlight arguments: guibg, guifg.
        /// </summary>
        Scrollbar,

        /// <summary>
        /// Current font, background and foreground of the tooltips. Applicable highlight arguments: font, guibg, guifg. NOTE: For Motif the font argument actually specifies a fontset at all times, no matter if 'guifontset' is empty, and as such it is tied to the current |:language| when set.
        /// </summary>
        Tooltip
    }
}