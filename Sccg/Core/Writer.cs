using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class Writer<TContent> : IWriter where TContent : IContent
{
    protected abstract void Write(IEnumerable<TContent> contents);

    public virtual int Priority => 0;

    void IWriter.Write(IEnumerable<IContent> contents)
    {
        Write(contents.OfType<TContent>());
    }
}