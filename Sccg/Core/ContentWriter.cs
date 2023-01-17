using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class ContentWriter<TContent> : IContentWriter where TContent : IContent
{
    protected abstract void Write(IEnumerable<TContent> contents);

    public virtual int Priority => 0;

    void IContentWriter.Write(IEnumerable<IContent> contents)
    {
        Write(contents.OfType<TContent>());
    }
}