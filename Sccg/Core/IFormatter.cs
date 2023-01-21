using System.Collections.Generic;

namespace Sccg.Core;

public interface IFormatter
{
    /// <summary>
    /// Source name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// TODO: doc
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="items"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public IContent Format(IEnumerable<ISourceItem> items, BuilderQuery query);
}