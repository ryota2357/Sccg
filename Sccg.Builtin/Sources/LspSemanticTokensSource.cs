using System;
using System.Collections.Generic;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for LSP semantic tokens.
/// </summary>
public class LspSemanticTokensSource : Source<LspSemanticTokensSource.Group, LspSemanticTokensSource.Item>
{
    private readonly StdSourceImpl<Group> _impl = new();

    /// <summary>
    /// Group of LSP semantic tokens.
    /// </summary>
    /// <param name="Type">SemanticTokenTypes</param>
    /// <param name="Modifier">SemanticTokenModifiers</param>
    public record struct Group(Type? Type, Modifier? Modifier)
    {
        /// <summary>
        /// Create a new instance of <see cref="Group"/> from <see cref="Type"/> and <see cref="Modifier"/>.
        /// </summary>
        public static implicit operator Group((Type type, Modifier modifier) value) => new(value.type, value.modifier);


        /// <summary>
        /// Create a new instance of <see cref="Group"/> from <see cref="Type"/>.
        /// </summary>
        public static implicit operator Group(Type type) => new(type, null);

        /// <summary>
        /// Create a new instance of <see cref="Group"/> from <see cref="Modifier"/>.
        /// </summary>
        public static implicit operator Group(Modifier modifier) => new(null, modifier);
    }

    /// <inheritdoc />
    public override string Name => "LspSemanticTokens";

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
                    {
                        Style? style = save.TryGetValue(toGroup, out var value) ? value : null;
                        if (style is not null)
                        {
                            save[fromGroup] = style.Value;
                        }
                        yield return new Item(fromGroup, toGroup, style);
                        break;
                    }
                    case Style style:
                    {
                        save[fromGroup] = style;
                        yield return new Item(fromGroup, style);
                        break;
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void Set(Group group, Style style) => _impl.Set(group, style);

    /// <inheritdoc />
    protected override void Link(Group from, Group to) => _impl.Link(from, to);

    /// <summary>
    /// SourceItem for LSP semantic tokens.
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
        public Item(Group group, Group link, Style? style = null)
        {
            _kind = Kind.Link;
            Group = group;
            Link = link;
            Style = style;
        }

        /// <inheritdoc />
        public NeovimFormatter.Formattable Extract()
        {
            return _kind switch
            {
                Kind.Set => new NeovimFormatter.Formattable
                {
                    Id = 0,
                    Name = CreateGroupString(Group),
                    Style = Style
                },
                Kind.Link => new NeovimFormatter.Formattable
                {
                    Id = 0,
                    Name = CreateGroupString(Group),
                    Link = CreateGroupString(Link!.Value)
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
    /// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#textDocument_semanticTokens
    /// <code>
    /// export enum SemanticTokenTypes {
    /// 	namespace = 'namespace',
    /// 	/**
    /// 	 * Represents a generic type. Acts as a fallback for types which
    /// 	 * can't be mapped to a specific type like class or enum.
    /// 	 */
    /// 	type = 'type',
    /// 	class = 'class',
    /// 	enum = 'enum',
    /// 	interface = 'interface',
    /// 	struct = 'struct',
    /// 	typeParameter = 'typeParameter',
    /// 	parameter = 'parameter',
    /// 	variable = 'variable',
    /// 	property = 'property',
    /// 	enumMember = 'enumMember',
    /// 	event = 'event',
    /// 	function = 'function',
    /// 	method = 'method',
    /// 	macro = 'macro',
    /// 	keyword = 'keyword',
    /// 	modifier = 'modifier',
    /// 	comment = 'comment',
    /// 	string = 'string',
    /// 	number = 'number',
    /// 	regexp = 'regexp',
    /// 	operator = 'operator',
    /// 	/**
    /// 	 * @since 3.17.0
    /// 	 */
    /// 	decorator = 'decorator'
    /// }
    /// </code>
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// SemanticTokenTypes.namespace
        /// </summary>
        Namespace,

        /// <summary>
        /// SemanticTokenTypes.type
        /// </summary>
        Type,

        /// <summary>
        /// SemanticTokenTypes.class
        /// </summary>
        Class,

        /// <summary>
        /// SemanticTokenTypes.enum
        /// </summary>
        Enum,

        /// <summary>
        /// SemanticTokenTypes.interface
        /// </summary>
        Interface,

        /// <summary>
        /// SemanticTokenTypes.struct
        /// </summary>
        Struct,

        /// <summary>
        /// SemanticTokenTypes.typeParameter
        /// </summary>
        TypeParameter,

        /// <summary>
        /// SemanticTokenTypes.parameter
        /// </summary>
        Parameter,

        /// <summary>
        /// SemanticTokenTypes.variable
        /// </summary>
        Variable,

        /// <summary>
        /// SemanticTokenTypes.property
        /// </summary>
        Property,

        /// <summary>
        /// SemanticTokenTypes.enumMember
        /// </summary>
        EnumMember,

        /// <summary>
        /// SemanticTokenTypes.event
        /// </summary>
        Event,

        /// <summary>
        /// SemanticTokenTypes.function
        /// </summary>
        Function,

        /// <summary>
        /// SemanticTokenTypes.method
        /// </summary>
        Method,

        /// <summary>
        /// SemanticTokenTypes.macro
        /// </summary>
        Macro,

        /// <summary>
        /// SemanticTokenTypes.keyword
        /// </summary>
        Keyword,

        /// <summary>
        /// SemanticTokenTypes.modifier
        /// </summary>
        Modifier,

        /// <summary>
        /// SemanticTokenTypes.comment
        /// </summary>
        Comment,

        /// <summary>
        /// SemanticTokenTypes.string
        /// </summary>
        String,

        /// <summary>
        /// SemanticTokenTypes.number
        /// </summary>
        Number,

        /// <summary>
        /// SemanticTokenTypes.regexp
        /// </summary>
        Regexp,

        /// <summary>
        /// SemanticTokenTypes.operator
        /// </summary>
        Operator,

        /// <summary>
        /// SemanticTokenTypes.decorator
        /// </summary>
        /// <remarks>@since 3.17.0</remarks>
        Decorator
    }

    /// <summary>
    /// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#textDocument_semanticTokens
    /// <code>
    /// export enum SemanticTokenModifiers {
    /// 	declaration = 'declaration',
    /// 	definition = 'definition',
    /// 	readonly = 'readonly',
    /// 	static = 'static',
    /// 	deprecated = 'deprecated',
    /// 	abstract = 'abstract',
    /// 	async = 'async',
    /// 	modification = 'modification',
    /// 	documentation = 'documentation',
    /// 	defaultLibrary = 'defaultLibrary'
    /// }
    /// </code>
    /// </summary>
    public enum Modifier
    {

        /// <summary>
        /// SemanticTokenModifiers.declaration
        /// </summary>
        Declaration,

        /// <summary>
        /// SemanticTokenModifiers.definition
        /// </summary>
        Definition,

        /// <summary>
        /// SemanticTokenModifiers.readonly
        /// </summary>
        Readonly,

        /// <summary>
        /// SemanticTokenModifiers.static
        /// </summary>
        Static,

        /// <summary>
        /// SemanticTokenModifiers.deprecated
        /// </summary>
        Deprecated,

        /// <summary>
        /// SemanticTokenModifiers.abstract
        /// </summary>
        Abstract,

        /// <summary>
        /// SemanticTokenModifiers.async
        /// </summary>
        Async,

        /// <summary>
        /// SemanticTokenModifiers.modification
        /// </summary>
        Modification,

        /// <summary>
        /// SemanticTokenModifiers.documentation
        /// </summary>
        Documentation,

        /// <summary>
        /// SemanticTokenModifiers.defaultLibrary
        /// </summary>
        DefaultLibrary
    }

    private static string CreateGroupString(Group group)
    {
        string StrT(Type? type) => type!.Value.ToString().ToLower();
        string StrM(Modifier? mod) => mod!.Value.ToString().ToLower();

        return group switch
        {
            { Type: not null, Modifier: not null } => $"@lsp.typemod.{StrM(group.Modifier)}.{StrT(group.Type)}",
            { Type: not null } => $"@lsp.type.{StrT(group.Type)}",
            { Modifier: not null } => $"@lsp.mod.{StrM(group.Modifier)}",
            _ => throw new ArgumentOutOfRangeException(nameof(group))
        };
    }
}