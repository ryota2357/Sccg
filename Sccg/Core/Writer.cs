using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

public abstract class Writer<TContent> : IWriter where TContent : IContent
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public virtual int Priority => 10;

    // TODO: doc
    protected virtual void Write(IEnumerable<TContent> contents)
    {
        throw new NotImplementedException("You must override Write method.");
    }

    // TODO: doc
    protected virtual void Write(IEnumerable<TContent> contents, BuilderQuery query)
    {
        Write(contents);
    }

    void IWriter.Write(IEnumerable<IContent> contents, BuilderQuery query)
    {
        Write(contents.OfType<TContent>());
    }
}