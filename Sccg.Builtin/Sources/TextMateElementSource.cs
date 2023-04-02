using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Sccg.Builtin.Develop;
using Sccg.Builtin.Formatters;
using Sccg.Core;

namespace Sccg.Builtin.Sources;

/// <summary>
/// Source for TextMate Element.
/// </summary>
public abstract partial class TextMateElementSource : Source<TextMateElementSource.Group, TextMateElementSource.Item>
{
    /// <inheritdoc />
    public override string Name => "TextMateElement";

    private readonly StdSourceImpl<Group> _impl = new();

    /// <inheritdoc />
    protected override IEnumerable<Item> CollectItems()
    {
        var ids = _impl.Graph.TopologicalOrderList();
        var save = new Dictionary<Group, Style>();
        foreach (var id in ids)
        {
            var data = _impl.Store.Load(id).data;
            var next = _impl.Graph.GetLink(id);

            if (data is not Group group || next is null)
            {
                continue;
            }

            var to = _impl.Store.Load(next.Value).data;
            switch (to)
            {
                case Style style:
                    save[group] = style;
                    yield return new Item(group, style);
                    break;
                case Group link:
                    if (!save.TryGetValue(link, out var linkStyle))
                    {
                        throw new InvalidDataException($"Group `{group}` does not have specific style.");
                    }
                    save[group] = linkStyle;
                    yield return new Item(group, linkStyle);
                    break;
            }
        }
    }

    /// <inheritdoc />
    protected override void Set(Group group, Style style) => _impl.Set(group, style);

    /// <inheritdoc />
    protected override void Link(Group from, Group to) => _impl.Link(from, to);

    /// <summary>
    /// SourceItem for TextMate Element.
    /// </summary>
    public class Item : IVSCodeTokenColorSourceItem
    {
        /// <summary>
        /// Gets the group.
        /// </summary>
        public readonly Group Group;

        /// <summary>
        /// Gets the style.
        /// </summary>
        public readonly Style Style;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item(Group group, Style style)
        {
            Group = group;
            Style = style;
        }

        /// <inheritdoc />
        public VSCodeFormatter.TokenColorFormattable Extract()
        {
            return new VSCodeFormatter.TokenColorFormattable
            {
                Name = CreateGroupString(Group),
                Style = Style
            };
        }
    }

    /// <summary>
    /// https://macromates.com/manual/en/language_grammars#naming_conventions
    /// </summary>
    public enum Group
    {
        /// <summary>
        /// comment ― for comments.
        /// </summary>
        Comment,

        /// <summary>
        /// comment.line ― line comments, we specialize further so that the type of comment start character(s) can be extracted from the scope.
        /// </summary>
        CommentLine,

        /// <summary>
        /// comment.line.double-slash ― // comment
        /// </summary>
        CommentLineDouble_slash,

        /// <summary>
        /// comment.line.double-dash ― -- comment
        /// </summary>
        CommentLineDouble_dash,

        /// <summary>
        /// comment.line.number-sign ― # comment
        /// </summary>
        CommentLineNumber_sign,

        /// <summary>
        /// comment.line.percentage ― % comment
        /// </summary>
        CommentLinePercentage,

        /// <summary>
        /// comment.line.character ― other types of line comments.
        /// </summary>
        CommentLineCharacter,

        /// <summary>
        /// comment.block ― multi-line comments like /* … */ and <!-- … -->.
        /// </summary>
        CommentBlock,

        /// <summary>
        /// comment.block.documentation ― embedded documentation.
        /// </summary>
        CommentBlockDocumentation,

        /// <summary>
        /// constant ― various forms of constants.
        /// </summary>
        Constant,

        /// <summary>
        /// constant.numeric ― those which represent numbers, e.g. 42, 1.3f, 0x4AB1U.
        /// </summary>
        ConstantNumeric,

        /// <summary>
        /// constant.character ― those which represent characters, e.g. &lt;, \e, \031.
        /// </summary>
        ConstantCharacter,

        /// <summary>
        /// constant.character.escape ― escape sequences like \e would be constant.character.escape.
        /// </summary>
        ConstantCharacterEscape,

        /// <summary>
        /// constant.language ― constants (generally) provided by the language which are “special” like true, false, nil, YES, NO, etc.
        /// </summary>
        ConstantLanguage,

        /// <summary>
        /// constant.other ― other constants, e.g. colors in CSS.
        /// </summary>
        ConstantOther,

        /// <summary>
        /// entity ― an entity refers to a larger part of the document, for example a chapter, class, function, or tag. We do not scope the entire entity as entity.* (we use meta.* for that). But we do use entity.* for the “placeholders” in the larger entity, e.g. if the entity is a chapter, we would use entity.name.section for the chapter title.
        /// </summary>
        Entity,

        /// <summary>
        /// entity.name ― we are naming the larger entity.
        /// </summary>
        EntityName,

        /// <summary>
        /// entity.name.function ― the name of a function.
        /// </summary>
        EntityNameFunction,

        /// <summary>
        /// entity.name.type ― the name of a type declaration or class.
        /// </summary>
        EntityNameType,

        /// <summary>
        /// entity.name.tag ― a tag name.
        /// </summary>
        EntityNameTag,

        /// <summary>
        /// entity.name.section ― the name is the name of a section/heading.
        /// </summary>
        EntityNameSection,

        /// <summary>
        /// entity.other ― other entities.
        /// </summary>
        EntityOther,

        /// <summary>
        /// entity.other.inherited-class ― the superclass/baseclass name.
        /// </summary>
        EntityOtherInherited_class,

        /// <summary>
        /// entity.other.attribute-name ― the name of an attribute (mainly in tags).
        /// </summary>
        EntityOtherAttribute_name,

        /// <summary>
        /// invalid ― stuff which is “invalid”.
        /// </summary>
        Invalid,

        /// <summary>
        /// invalid.illegal ― illegal, e.g. an ampersand or lower-than character in HTML (which is not part of an entity/tag).
        /// </summary>
        InvalidIllegal,

        /// <summary>
        /// invalid.deprecated ― for deprecated stuff e.g. using an API function which is deprecated or using styling with strict HTML.
        /// </summary>
        InvalidDeprecated,

        /// <summary>
        /// keyword ― keywords (when these do not fall into the other groups).
        /// </summary>
        Keyword,

        /// <summary>
        /// keyword.control ― mainly related to flow control like continue, while, return, etc.
        /// </summary>
        KeywordControl,

        /// <summary>
        /// keyword.operator ― operators can either be textual (e.g. or) or be characters.
        /// </summary>
        KeywordOperator,

        /// <summary>
        /// keyword.other ― other keywords.
        /// </summary>
        KeywordOther,

        /// <summary>
        /// markup ― this is for markup languages and generally applies to larger subsets of the text.
        /// </summary>
        Markup,

        /// <summary>
        /// markup.underline ― underlined text.
        /// </summary>
        MarkupUnderline,

        /// <summary>
        /// markup.underline.link ― this is for links, as a convenience this is derived from markup.underline so that if there is no theme rule which specifically targets markup.underline.link then it will inherit the underline style.
        /// </summary>
        MarkupUnderlineLink,

        /// <summary>
        /// markup.bold ― bold text (text which is strong and similar should preferably be derived from this name).
        /// </summary>
        MarkupBold,

        /// <summary>
        /// markup.heading ― a section header. Optionally provide the heading level as the next element, for example markup.heading.2.html for <h2>…</h2> in HTML.
        /// </summary>
        MarkupHeading,

        /// <summary>
        /// markup.italic ― italic text (text which is emphasized and similar should preferably be derived from this name).
        /// </summary>
        MarkupItalic,

        /// <summary>
        /// markup.list ― list items.
        /// </summary>
        MarkupList,

        /// <summary>
        /// markup.list.numbered ― numbered list items.
        /// </summary>
        MarkupListNumbered,

        /// <summary>
        /// markup.list.unnumbered ― unnumbered list items.
        /// </summary>
        MarkupListUnnumbered,

        /// <summary>
        /// markup.quote ― quoted (sometimes block quoted) text.
        /// </summary>
        MarkupQuote,

        /// <summary>
        /// markup.raw ― text which is verbatim, e.g. code listings. Normally spell checking is disabled for markup.raw.
        /// </summary>
        MarkupRaw,

        /// <summary>
        /// markup.other ― other markup constructs.
        /// </summary>
        MarkupOther,

        /// <summary>
        /// meta ― the meta scope is generally used to markup larger parts of the document. For example the entire line which declares a function would be meta.function and the subsets would be storage.type, entity.name.function, variable.parameter etc. and only the latter would be styled. Sometimes the meta part of the scope will be used only to limit the more general element that is styled, most of the time meta scopes are however used in scope selectors for activation of bundle items. For example in Objective-C there is a meta scope for the interface declaration of a class and the implementation, allowing the same tab-triggers to expand differently, depending on context.
        /// </summary>
        Meta,

        /// <summary>
        /// storage ― things relating to “storage”.
        /// </summary>
        Storage,

        /// <summary>
        /// storage.type ― the type of something, class, function, int, var, etc.
        /// </summary>
        StorageType,

        /// <summary>
        /// storage.modifier ― a storage modifier like static, final, abstract, etc.
        /// </summary>
        StorageModifier,

        /// <summary>
        /// string ― strings.
        /// </summary>
        String,

        /// <summary>
        /// string.quoted ― quoted strings.
        /// </summary>
        StringQuoted,

        /// <summary>
        /// string.quoted.single ― single quoted strings: 'foo'.
        /// </summary>
        StringQuotedSingle,

        /// <summary>
        /// string.quoted.double ― double quoted strings: "foo".
        /// </summary>
        StringQuotedDouble,

        /// <summary>
        /// string.quoted.triple ― triple quoted strings: """Python""".
        /// </summary>
        StringQuotedTriple,

        /// <summary>
        /// string.quoted.other ― other types of quoting: $'shell', %s{...}.
        /// </summary>
        StringQuotedOther,

        /// <summary>
        /// string.unquoted ― for things like here-docs and here-strings.
        /// </summary>
        StringUnquoted,

        /// <summary>
        /// string.interpolated ― strings which are “evaluated”: `date`, $(pwd).
        /// </summary>
        StringInterpolated,

        /// <summary>
        /// string.regexp ― regular expressions: /(\w+)/.
        /// </summary>
        StringRegexp,

        /// <summary>
        /// string.other ― other types of strings (should rarely be used).
        /// </summary>
        StringOther,

        /// <summary>
        /// support ― things provided by a framework or library should be below support.
        /// </summary>
        Support,

        /// <summary>
        /// support.function ― functions provided by the framework/library. For example NSLog in Objective-C is support.function.
        /// </summary>
        SupportFunction,

        /// <summary>
        /// support.class ― when the framework/library provides classes.
        /// </summary>
        SupportClass,

        /// <summary>
        /// support.type ― types provided by the framework/library, this is probably only used for languages derived from C, which has typedef (and struct). Most other languages would introduce new types as classes.
        /// </summary>
        SupportType,

        /// <summary>
        /// support.constant ― constants (magic values) provided by the framework/library.
        /// </summary>
        SupportConstant,

        /// <summary>
        /// support.variable ― variables provided by the framework/library. For example NSApp in AppKit.
        /// </summary>
        SupportVariable,

        /// <summary>
        /// support.other ― the above should be exhaustive, but for everything else use support.other.
        /// </summary>
        SupportOther,

        /// <summary>
        /// variable ― variables. Not all languages allow easy identification (and thus markup) of these.
        /// </summary>
        Variable,

        /// <summary>
        /// variable.parameter ― when the variable is declared as the parameter.
        /// </summary>
        VariableParameter,

        /// <summary>
        /// variable.language ― reserved language variables like this, super, self, etc.
        /// </summary>
        VariableLanguage,

        /// <summary>
        /// variable.other ― other variables, like $some_variables.
        /// </summary>
        VariableOther,
    }

    private static string CreateGroupString(Group group)
    {
        var dottedName = UpperCharRegex().Replace(group.ToString(), ".$1").ToLower()[1..];
        var name = dottedName.Replace('_', '-');
        return name;
    }

    [GeneratedRegex("([A-Z])")]
    private static partial Regex UpperCharRegex();
}