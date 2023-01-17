using System.Collections.Generic;

namespace Sccg.Core;

public interface IContentWriter
{
    /// <summary>
    /// TODO: doc
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// TODO: doc
    /// </summary>
    public void Write(IEnumerable<IContent> contents);
}