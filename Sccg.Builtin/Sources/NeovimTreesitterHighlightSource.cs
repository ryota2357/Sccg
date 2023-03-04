using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for Neovim Treesitter highlight.
/// </summary>
public abstract partial class NeovimTreesitterHighlightSource
    : Source<NeovimTreesitterHighlightSource.Group, NeovimTreesitterHighlightSource.Item>
{
    private record FiletypedGroup(Group Name, string? Filetype)
    {
        public (Group name, string? filetyp) Deconstruct() => (Name, Filetype);
        public static implicit operator FiletypedGroup((Group name, string? filetyp) value) => new(value.name, value.filetyp);
    }

    private readonly StdSourceImpl<FiletypedGroup> _impl = new();
    private string? _filetype;

    /// <inheritdoc />
    public override string Name => "NeovimTreesitterHighlight";

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();
        var save = new Dictionary<FiletypedGroup, Style>();

        foreach (var id in ids)
        {
            var data = _impl.Store.Load(id).data;
            var next = _impl.Graph.GetLink(id);

            if (data is FiletypedGroup fromGroup && next is not null)
            {
                var to = _impl.Store.Load(next.Value).data;
                switch (to)
                {
                    case FiletypedGroup toGroup:
                        Style? sty = save.ContainsKey(toGroup) ? save[toGroup] : null;
                        if (sty is not null)
                        {
                            save[fromGroup] = sty.Value;
                        }
                        yield return new Item(fromGroup.Deconstruct(), toGroup.Deconstruct(), sty);
                        break;
                    case Style style:
                        save[fromGroup] = style;
                        yield return new Item(fromGroup.Deconstruct(), style);
                        break;
                    case VimSyntaxGroupSource.Group vimSyntaxGroup:
                        yield return new Item(fromGroup.Deconstruct(), vimSyntaxGroup);
                        break;
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void Set(Group group, Style style)
    {
        _impl.Set((group, _filetype), style);
    }

    /// <inheritdoc />
    /// <remarks>
    /// If you want to link to filetype specific group, use <see cref="Link(Group, Group, string)"/>.
    /// </remarks>
    protected override void Link(Group from, Group to)
    {
        _impl.Link((from, _filetype), (to, null));
    }

    /// <summary>
    /// Link `from` group to `to` filetype specific group.
    /// </summary>
    /// <param name="from">A syntax/design group name.</param>
    /// <param name="to">A syntax/design group name.</param>
    /// <param name="filetype">A filetype name.</param>
    protected void Link(Group from, Group to, string filetype)
    {
        _impl.Link((from, _filetype), (to, filetype));
    }

    /// <summary>
    /// Custom for the specified filetype.
    /// </summary>
    /// <exception cref="InvalidOperationException">Attempt to enter a nested filetype section.</exception>
    protected void Filetype(string filetype, Action custom)
    {
        if (_filetype is not null)
        {
            throw new InvalidOperationException("Already in filetype section.");
        }
        _filetype = filetype;
        custom();
        _filetype = null;
    }

    /// <inheritdoc cref="Link(Sccg.Builtin.Sources.NeovimTreesitterHighlightSource.Group,Sccg.Builtin.Sources.NeovimTreesitterHighlightSource.Group)"/>
    protected void Link(Group from, VimSyntaxGroupSource.Group to)
    {
        var fromId = _impl.Store.Save(from);
        var toId = _impl.Store.Save(to);
        var status = _impl.Graph.CreateLink(fromId, toId);
        if (status == false)
        {
            Log.Warn($"Ignored duplicate. Link({from}, {to})");
        }
    }

    /// <summary>
    /// SourceItem for Neovim Treesitter highlight.
    /// </summary>
    public class Item : INeovimSourceItem
    {
        private readonly Kind _kind;
        private readonly FiletypedGroup _group;
        private readonly FiletypedGroup? _link;

        /// <summary>
        /// Gets the group to set.
        /// </summary>
        public (Group name, string? filetype) Group => _group.Deconstruct();

        /// <summary>
        /// Gets the style to set.
        /// </summary>
        public readonly Style? Style;

        /// <summary>
        /// Gets the group to link.
        /// </summary>
        public (Group name, string? filetype)? Link => _link?.Deconstruct();

        /// <summary>
        /// Gets the vim syntax group to link.
        /// </summary>
        public readonly VimSyntaxGroupSource.Group? LinkVimSyntaxGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item((Group, string?) group, Style style)
        {
            _kind = Kind.Set;
            _group = group;
            Style = style;
            _link = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item((Group, string?) from, (Group, string?) to, Style? style = null)
        {
            _kind = Kind.Link;
            _group = from;
            Style = style;
            _link = to;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item((Group, string?) from, VimSyntaxGroupSource.Group link)
        {
            _kind = Kind.VimSyntaxLink;
            _group = from;
            Style = null;
            _link = null;
            LinkVimSyntaxGroup = link;
        }

        /// <inheritdoc />
        public NeovimFormatter.Formattable Extract()
        {
            return _kind switch
            {
                Kind.Set => new NeovimFormatter.Formattable
                {
                    Name = CreateGroupString(_group),
                    Id = 0,
                    Style = Style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Name = CreateGroupString(_group),
                    Id = 0,
                    Link = CreateGroupString(_link!)
                },
                Kind.VimSyntaxLink => new NeovimFormatter.Formattable
                {
                    Name = CreateGroupString(_group),
                    Id = 0,
                    Link = LinkVimSyntaxGroup!.Value.ToString()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum Kind
        {
            Set,
            Link,
            VimSyntaxLink,
        }
    }

    /// <summary>
    /// https://github.com/nvim-treesitter/nvim-treesitter/blob/master/CONTRIBUTING.md#highlights
    /// </summary>
    /// <remarks>Some groups are not supported by neovim.</remarks>
    public enum Group
    {

        /// <summary>
        /// @comment ; line and block comments
        /// </summary>
        Comment,

        /// <summary>
        /// @error ; syntax/parser errors
        /// </summary>
        Error,

        /// <summary>
        /// @none ; completely disable the highlight
        /// </summary>
        None,

        /// <summary>
        /// @preproc ; various preprocessor directives &amp; shebangs
        /// </summary>
        Preproc,

        /// <summary>
        /// @define ; preprocessor definition directives
        /// </summary>
        Define,

        /// <summary>
        /// @operator ; symbolic operators (e.g. `+` / `*`)
        /// </summary>
        Operator,

        /// <summary>
        /// @punctuation.delimiter ; delimiters (e.g. `;` / `.` / `,`)
        /// </summary>
        PunctuationDelimiter,

        /// <summary>
        /// @punctuation.bracket ; brackets (e.g. `()` / `{}` / `[]`)
        /// </summary>
        PunctuationBracket,

        /// <summary>
        /// @punctuation.special ; special symbols (e.g. `{}` in string interpolation)
        /// </summary>
        PunctuationSpecial,

        /// <summary>
        /// @string ; string literals
        /// </summary>
        String,

        /// <summary>
        /// @string.regex ; regular expressions
        /// </summary>
        StringRegex,

        /// <summary>
        /// @string.escape ; escape sequences
        /// </summary>
        StringEscape,

        /// <summary>
        /// @string.special ; other special strings (e.g. dates)
        /// </summary>
        StringSpecial,

        /// <summary>
        /// @character ; character literals
        /// </summary>
        Character,

        /// <summary>
        /// @character.special ; special characters (e.g. wildcards)
        /// </summary>
        CharacterSpecial,

        /// <summary>
        /// @boolean ; boolean literals
        /// </summary>
        Boolean,

        /// <summary>
        /// @number ; numeric literals
        /// </summary>
        Number,

        /// <summary>
        /// @float ; floating-point number literals
        /// </summary>
        Float,

        /// <summary>
        /// @function ; function definitions
        /// </summary>
        Function,

        /// <summary>
        /// @function.builtin ; built-in functions
        /// </summary>
        FunctionBuiltin,

        /// <summary>
        /// @function.call ; function calls
        /// </summary>
        FunctionCall,

        /// <summary>
        /// @function.macro ; preprocessor macros
        /// </summary>
        FunctionMacro,

        /// <summary>
        /// @method ; method definitions
        /// </summary>
        Method,

        /// <summary>
        /// @method.call ; method calls
        /// </summary>
        MethodCall,

        /// <summary>
        /// @constructor ; constructor calls and definitions
        /// </summary>
        Constructor,

        /// <summary>
        /// @parameter ; parameters of a function
        /// </summary>
        Parameter,

        /// <summary>
        /// @keyword ; various keywords
        /// </summary>
        Keyword,

        /// <summary>
        /// @keyword.function ; keywords that define a function (e.g. `func` in Go, `def` in Python)
        /// </summary>
        KeywordFunction,

        /// <summary>
        /// @keyword.operator ; operators that are English words (e.g. `and` / `or`)
        /// </summary>
        KeywordOperator,

        /// <summary>
        /// @keyword.return ; keywords like `return` and `yield`
        /// </summary>
        KeywordReturn,

        /// <summary>
        /// @conditional ; keywords related to conditionals (e.g. `if` / `else`)
        /// </summary>
        Conditional,

        /// <summary>
        /// @conditional.ternary ; ternary operator (e.g. `?` / `:`)
        /// </summary>
        ConditionalTernary,

        /// <summary>
        /// @repeat ; keywords related to loops (e.g. `for` / `while`)
        /// </summary>
        Repeat,

        /// <summary>
        /// @debug ; keywords related to debugging
        /// </summary>
        Debug,

        /// <summary>
        /// @label ; GOTO and other labels (e.g. `label:` in C)
        /// </summary>
        Label,

        /// <summary>
        /// @include ; keywords for including modules (e.g. `import` / `from` in Python)
        /// </summary>
        Include,

        /// <summary>
        /// @exception ; keywords related to exceptions (e.g. `throw` / `catch`)
        /// </summary>
        Exception,

        /// <summary>
        /// @type ; type or class definitions and annotations
        /// </summary>
        Type,

        /// <summary>
        /// @type.builtin ; built-in types
        /// </summary>
        TypeBuiltin,

        /// <summary>
        /// @type.definition ; type definitions (e.g. `typedef` in C)
        /// </summary>
        TypeDefinition,

        /// <summary>
        /// @type.qualifier ; type qualifiers (e.g. `const`)
        /// </summary>
        TypeQualifier,

        /// <summary>
        /// @storageclass ; modifiers that affect storage in memory or life-time
        /// </summary>
        Storageclass,

        /// <summary>
        /// @attribute ; attribute annotations (e.g. Python decorators)
        /// </summary>
        Attribute,

        /// <summary>
        /// @field ; object and struct fields
        /// </summary>
        Field,

        /// <summary>
        /// @property ; similar to `@field`
        /// </summary>
        Property,

        /// <summary>
        /// @variable ; various variable names
        /// </summary>
        Variable,

        /// <summary>
        /// @variable.builtin ; built-in variable names (e.g. `this`)
        /// </summary>
        VariableBuiltin,

        /// <summary>
        /// @constant ; constant identifiers
        /// </summary>
        Constant,

        /// <summary>
        /// @constant.builtin ; built-in constant values
        /// </summary>
        ConstantBuiltin,

        /// <summary>
        /// @constant.macro ; constants defined by the preprocessor
        /// </summary>
        ConstantMacro,

        /// <summary>
        /// @namespace ; modules or namespaces
        /// </summary>
        Namespace,

        /// <summary>
        /// @symbol ; symbols or atoms
        /// </summary>
        Symbol,

        /// <summary>
        /// @text ; non-structured text
        /// </summary>
        Text,

        /// <summary>
        /// @text.strong ; bold text
        /// </summary>
        TextStrong,

        /// <summary>
        /// @text.emphasis ; text with emphasis
        /// </summary>
        TextEmphasis,

        /// <summary>
        /// @text.underline ; underlined text
        /// </summary>
        TextUnderline,

        /// <summary>
        /// @text.strike ; strikethrough text
        /// </summary>
        TextStrike,

        /// <summary>
        /// @text.title ; text that is part of a title
        /// </summary>
        TextTitle,

        /// <summary>
        /// @text.literal ; literal or verbatim text (e.g., inline code)
        /// </summary>
        TextLiteral,

        /// <summary>
        /// @text.quote ; text quotations
        /// </summary>
        TextQuote,

        /// <summary>
        /// @text.uri ; URIs (e.g. hyperlinks)
        /// </summary>
        TextUri,

        /// <summary>
        /// @text.math ; math environments (e.g. `$ ... $` in LaTeX)
        /// </summary>
        TextMath,

        /// <summary>
        /// @text.environment ; text environments of markup languages
        /// </summary>
        TextEnvironment,

        /// <summary>
        /// @text.environment.name ; text indicating the type of an environment
        /// </summary>
        TextEnvironmentName,

        /// <summary>
        /// @text.reference ; text references, footnotes, citations, etc.
        /// </summary>
        TextReference,

        /// <summary>
        /// @text.todo ; todo notes
        /// </summary>
        TextTodo,

        /// <summary>
        /// @text.note ; info notes
        /// </summary>
        TextNote,

        /// <summary>
        /// @text.warning ; warning notes
        /// </summary>
        TextWarning,

        /// <summary>
        /// @text.danger ; danger/error notes
        /// </summary>
        TextDanger,

        /// <summary>
        /// @text.diff.add ; added text (for diff files)
        /// </summary>
        TextDiffAdd,

        /// <summary>
        /// @text.diff.delete ; deleted text (for diff files)
        /// </summary>
        TextDiffDelete,

        /// <summary>
        /// @tag ; XML tag names
        /// </summary>
        Tag,

        /// <summary>
        /// @tag.attribute ; XML tag attributes
        /// </summary>
        TagAttribute,

        /// <summary>
        /// @tag.delimiter ; XML tag delimiters
        /// </summary>
        TagDelimiter,

        /// <summary>
        /// @conceal ; for captures that are only used for concealing
        /// </summary>
        Conceal,

        /// <summary>
        /// @spell ; for defining regions to be spellchecked
        /// </summary>
        Spell,

        /// <summary>
        /// @nospell ; for defining regions that should NOT be spellchecked
        /// </summary>
        Nospell,
    }

    private static string CreateGroupString(FiletypedGroup group)
    {
        var name = $"@{UpperCharRegex().Replace(group.Name.ToString(), ".$1").ToLower()[1..]}";
        var ft = group.Filetype is null ? "" : $".{group.Filetype}";
        return $"{name}{ft}";
    }

    [GeneratedRegex("([A-Z])")]
    private static partial Regex UpperCharRegex();
}