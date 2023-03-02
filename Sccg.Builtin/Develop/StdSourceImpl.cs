using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Develop;

/// <summary>
/// A standard implementation of <see cref="ISource"/>.
/// </summary>
public class StdSourceImpl<TGroup>
    where TGroup : notnull
{
    /// <summary>
    /// Gets the instance of <see cref="ObjectStore"/>.
    /// </summary>
    public readonly ObjectStore Store = new();

    /// <summary>
    /// Gets the instance of <see cref="SingeLinkGraph"/>.
    /// </summary>
    public readonly SingeLinkGraph Graph = new();

    /// <summary>
    /// Sets the style of the specified group.
    /// </summary>
    public void Set(TGroup group, Style style)
    {
        var groupId = Store.Save(group);
        var styleId = Store.Save(style);
        var status = Graph.CreateLink(groupId, styleId);
        if (status == false)
        {
            Log.Warn($"Ignored duplicate. Set({group}, {style})");
        }
    }

    /// <summary>
    /// Sets the color of the specified group.
    /// </summary>
    public void Set(TGroup group, Color color)
    {
        var groupId = Store.Save(group);
        var styleId = Store.Save(color);
        var status = Graph.CreateLink(groupId, styleId);
        if (status == false)
        {
            Log.Warn($"Ignored duplicate. Set({group}, {color})");
        }
    }

    /// <summary>
    /// Links the specified groups.
    /// </summary>
    public void Link(TGroup from, TGroup to)
    {
        var fromId = Store.Save(from);
        var toId = Store.Save(to);
        var status = Graph.CreateLink(fromId, toId);
        if (status == false)
        {
            Log.Warn($"Ignored duplicate. Link({from}, {to})");
        }
    }
}