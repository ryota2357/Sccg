using System.Collections.Generic;

namespace Sccg.Core;

public interface IWriter
{
    /// <summary>
    /// TODO: doc
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// TODO: doc
    /// </summary>
    public void Write(IEnumerable<IContent> contents, BuilderQuery query);
}