using System.Collections.Generic;
using Sccg.Builtin.Writers;
using Sccg.Core;

namespace Sccg.Builtin.Converters;

public class MultiTextContentSplitter : IContentConverter
{
    public string Name => "MultiTextContentSplitter";
    public int Priority => 10;

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