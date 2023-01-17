using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

public abstract partial class NeovimTreesitterHighlightSource
    : Source<NeovimTreesitterHighlightSource.Group, NeovimTreesitterHighlightSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    public override string Name => "NeovimTreesitterHighlight";

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
        private readonly Group _fromGroup;
        private readonly Style? _style;
        private readonly Group? _toGroup;

        public Item(Group group, Style style)
        {
            _kind = Kind.Set;
            _fromGroup = group;
            _style = style;
            _toGroup = null;
        }

        public Item(Group from, Group to)
        {
            _kind = Kind.Link;
            _fromGroup = from;
            _style = null;
            _toGroup = to;
        }

        public NeovimFormatter.Formattable Extract()
        {
            return _kind switch
            {
                Kind.Set => new NeovimFormatter.Formattable
                {
                    Name = CreateGroupString(_fromGroup),
                    Id = 0,
                    Style = _style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Name = CreateGroupString(_fromGroup),
                    Id = 0,
                    Link = CreateGroupString(_toGroup!.Value)
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
    /// https://github.com/nvim-treesitter/nvim-treesitter/blob/master/CONTRIBUTING.md#highlights
    /// </summary>
    public enum Group
    {
        /// <summary>
        /// line and block comments
        /// </summary>
        Comment,

        /// <summary>
        /// syntax/parser errors
        /// </summary>
        Error,

        /// <summary>
        /// completely disable the highlight
        /// </summary>
        None,

        /// <summary>
        /// various preprocessor directives & shebangs
        /// </summary>
        Preproc,

        /// <summary>
        /// preprocessor definition directives
        /// </summary>
        Define,

        /// <summary>
        /// symbolic operators (e.g. `+` / `*`)
        /// </summary>
        Operator,

        /// <summary>
        /// delimiters (e.g. `
        /// </summary>
        PunctuationDelimiter,

        /// <summary>
        /// brackets (e.g. `()` / `{}` / `[]`)
        /// </summary>
        PunctuationBracket,

        /// <summary>
        /// special symbols (e.g. `{}` in string interpolation)
        /// </summary>
        PunctuationSpecial,

        /// <summary>
        /// string literals
        /// </summary>
        String,

        /// <summary>
        /// regular expressions
        /// </summary>
        StringRegex,

        /// <summary>
        /// escape sequences
        /// </summary>
        StringEscape,

        /// <summary>
        /// other special strings (e.g. dates)
        /// </summary>
        StringSpecial,

        /// <summary>
        /// character literals
        /// </summary>
        Character,

        /// <summary>
        /// special characters (e.g. wildcards)
        /// </summary>
        CharacterSpecial,

        /// <summary>
        /// boolean literals
        /// </summary>
        Boolean,

        /// <summary>
        /// numeric literals
        /// </summary>
        Number,

        /// <summary>
        /// floating-point number literals
        /// </summary>
        Float,

        /// <summary>
        /// function definitions
        /// </summary>
        Function,

        /// <summary>
        /// built-in functions
        /// </summary>
        FunctionBuiltin,

        /// <summary>
        /// function calls
        /// </summary>
        FunctionCall,

        /// <summary>
        /// preprocessor macros
        /// </summary>
        FunctionMacro,

        /// <summary>
        /// method definitions
        /// </summary>
        Method,

        /// <summary>
        /// method calls
        /// </summary>
        MethodCall,

        /// <summary>
        /// constructor calls and definitions
        /// </summary>
        Constructor,

        /// <summary>
        /// parameters of a function
        /// </summary>
        Parameter,

        /// <summary>
        /// various keywords
        /// </summary>
        Keyword,

        /// <summary>
        /// keywords that define a function (e.g. `func` in Go, `def` in Python)
        /// </summary>
        KeywordFunction,

        /// <summary>
        /// operators that are English words (e.g. `and` / `or`)
        /// </summary>
        KeywordOperator,

        /// <summary>
        /// keywords like `return` and `yield`
        /// </summary>
        KeywordReturn,

        /// <summary>
        /// keywords related to conditionals (e.g. `if` / `else`)
        /// </summary>
        Conditional,

        /// <summary>
        /// Ternary operator: condition ? 1 : 2
        /// </summary>
        ConditionalTernary,

        /// <summary>
        /// keywords related to loops (e.g. `for` / `while`)
        /// </summary>
        Repeat,

        /// <summary>
        /// keywords related to debugging
        /// </summary>
        Debug,

        /// <summary>
        /// GOTO and other labels (e.g. `label:` in C)
        /// </summary>
        Label,

        /// <summary>
        /// keywords for including modules (e.g. `import` / `from` in Python)
        /// </summary>
        Include,

        /// <summary>
        /// keywords related to exceptions (e.g. `throw` / `catch`)
        /// </summary>
        Exception,

        /// <summary>
        /// type or class definitions and annotations
        /// </summary>
        Type,

        /// <summary>
        /// built-in types
        /// </summary>
        TypeBuiltin,

        /// <summary>
        /// type definitions (e.g. `typedef` in C)
        /// </summary>
        TypeDefinition,

        /// <summary>
        /// type qualifiers (e.g. `const`)
        /// </summary>
        TypeQualifier,

        /// <summary>
        /// visibility/life-time modifiers
        /// </summary>
        Storageclass,

        /// <summary>
        /// attribute annotations (e.g. Python decorators)
        /// </summary>
        Attribute,

        /// <summary>
        /// object and struct fields
        /// </summary>
        Field,

        /// <summary>
        /// similar to `@field`
        /// </summary>
        Property,

        /// <summary>
        /// various variable names
        /// </summary>
        Variable,

        /// <summary>
        /// built-in variable names (e.g. `this`)
        /// </summary>
        VariableBuiltin,

        /// <summary>
        /// constant identifiers
        /// </summary>
        Constant,

        /// <summary>
        /// built-in constant values
        /// </summary>
        ConstantBuiltin,

        /// <summary>
        /// constants defined by the preprocessor
        /// </summary>
        ConstantMacro,

        /// <summary>
        /// modules or namespaces
        /// </summary>
        Namespace,

        /// <summary>
        /// symbols or atoms
        /// </summary>
        Symbol,

        /// <summary>
        /// non-structured text
        /// </summary>
        Text,

        /// <summary>
        /// bold text
        /// </summary>
        TextStrong,

        /// <summary>
        /// text with emphasis
        /// </summary>
        TextEmphasis,

        /// <summary>
        /// underlined text
        /// </summary>
        TextUnderline,

        /// <summary>
        /// strikethrough text
        /// </summary>
        TextStrike,

        /// <summary>
        /// text that is part of a title
        /// </summary>
        TextTitle,

        /// <summary>
        /// literal or verbatim text (e.g., inline code)
        /// </summary>
        TextLiteral,

        /// <summary>
        /// text quotations
        /// </summary>
        TextQuote,

        /// <summary>
        /// URIs (e.g. hyperlinks)
        /// </summary>
        TextUri,

        /// <summary>
        /// math environments (e.g. `$ ... $` in LaTeX)
        /// </summary>
        TextMath,

        /// <summary>
        /// text environments of markup languages
        /// </summary>
        TextEnvironment,

        /// <summary>
        /// text indicating the type of an environment
        /// </summary>
        TextEnvironmentName,

        /// <summary>
        /// text references, footnotes, citations, etc.
        /// </summary>
        TextReference,

        /// <summary>
        /// todo notes
        /// </summary>
        TextTodo,

        /// <summary>
        /// info notes
        /// </summary>
        TextNote,

        /// <summary>
        /// warning notes
        /// </summary>
        TextWarning,

        /// <summary>
        /// danger/error notes
        /// </summary>
        TextDanger,

        /// <summary>
        /// added text (for diff files)
        /// </summary>
        TextDiffAdd,

        /// <summary>
        /// deleted text (for diff files)
        /// </summary>
        TextDiffDelete,

        /// <summary>
        /// XML tag names
        /// </summary>
        Tag,

        /// <summary>
        /// XML tag attributes
        /// </summary>
        TagAttribute,

        /// <summary>
        /// XML tag delimiters
        /// </summary>
        TagDelimiter,

        /// <summary>
        /// for captures that are only used for concealing
        /// </summary>
        Conceal,

        /// <summary>
        /// for defining regions to be spellchecked
        /// </summary>
        Spell,

        /// <summary>
        /// for defining regions that should NOT be spellchecked
        /// </summary>
        Nospell

    }

    private static string CreateGroupString(Group group)
    {
        return $"@{UpperCharRegex().Replace(group.ToString(), ".$1").ToLower()[1..]}";
    }

    [GeneratedRegex("([A-Z])")]
    private static partial Regex UpperCharRegex();
}