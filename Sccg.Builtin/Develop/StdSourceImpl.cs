using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Develop;

public class StdSourceImpl<TGroup>
    where TGroup : notnull
{
    public readonly ObjectStore Store = new();
    public readonly SingeLinkGraph Graph = new();

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