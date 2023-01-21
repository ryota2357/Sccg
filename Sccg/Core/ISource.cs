using System.Collections.Generic;

namespace Sccg.Core;

public interface ISource
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
    /// Collects source items from the source.
    /// </summary>
    /// <returns>The collection of <see cref="ISourceItem"/></returns>
    public IEnumerable<ISourceItem> CollectItems();

    /// <summary>
    /// You can set styles to source items (Group) here.
    /// </summary>
    /// <param name="query">TODO: doc</param>
    /// <remarks>This method is for users, should not override in abstract Source class.</remarks>
    public void Custom(BuilderQuery query);
}