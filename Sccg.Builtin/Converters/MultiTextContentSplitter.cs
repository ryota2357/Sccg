using System.Collections.Generic;
using Sccg.Builtin.Writers;
using Sccg.Core;

namespace Sccg.Builtin.Converters;

/// <summary>
/// Converter (ContentConverter) that splits <see cref="MultiTextContent"/> into <see cref="SingleTextContent"/>.
/// </summary>
public class MultiTextContentSplitter : IContentConverter
{
    /// <inheritdoc />
    public string Name => "MultiTextContentSplitter";

    /// <inheritdoc />
    public int Priority => 10;

    /// <inheritdoc />
    public IEnumerable<IContent> Convert(List<IContent> contents, BuilderQuery query)
    {
        foreach (var content in contents)
        {
            if (content is MultiTextContent multiTextContent)
            {
                foreach (var singleTextContent in multiTextContent.ToSingleTextContents())
                {
                    yield return singleTextContent;
                }
            }
            else
            {
                yield return content;
            }
        }
    }
}