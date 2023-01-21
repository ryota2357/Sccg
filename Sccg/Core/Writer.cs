using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class Writer<TContent> : IWriter where TContent : IContent
{
    public virtual int Priority => 10;

    protected virtual void Write(IEnumerable<TContent> contents)
    {
        throw new NotImplementedException("You must override Write method.");
    }

    protected virtual void Write(IEnumerable<TContent> contents, BuilderQuery query)
    {
        Write(contents);
    }

    void IWriter.Write(IEnumerable<IContent> contents, BuilderQuery query)
    {
        Write(contents.OfType<TContent>());
    }
}