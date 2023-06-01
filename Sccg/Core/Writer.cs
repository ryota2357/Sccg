using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Core;

/// <summary>
/// The base class of <see cref="IContent"/>.
/// </summary>
public abstract class Writer<TContent> : IWriter where TContent : IContent
{
    /// <inheritdoc cref="IWriter.Name"/>
    public abstract string Name { get; }

    /// <inheritdoc cref="IWriter.Priority"/>
    public virtual int Priority => 10;

    /// <summary>
    /// Writes the specified content.
    /// </summary>
    /// <remarks>This method is called only once.</remarks>
    protected virtual void Write(IEnumerable<TContent> contents)
    {
        throw new NotImplementedException("You must override Write method.");
    }

    /// <inheritdoc cref="Write(System.Collections.Generic.IEnumerable{TContent})"/>
    protected virtual void Write(IEnumerable<TContent> contents, BuilderQuery query)
    {
        Write(contents);
    }

    void IWriter.Write(IEnumerable<IContent> contents, BuilderQuery query)
    {
        Write(contents.OfType<TContent>(), query);
    }
}