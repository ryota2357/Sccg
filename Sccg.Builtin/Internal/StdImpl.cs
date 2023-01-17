using System;
using System.Collections.Generic;
using System.IO;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Internal;

public static class StdImpl
{
    public static class Source
    {
        public static void Set<TGroup>(in ObjectStore store, in SingeLinkGraph graph, in TGroup group, in Style style)
            where TGroup : notnull
        {
            var groupId = store.Save(group);
            var styleId = store.Save(style);
            var status = graph.CreateLink(groupId, styleId);
            if (status == false)
            {
                Log.Warn($"Ignored duplicate. Set({group}, {style})");
            }
        }

        public static void Link<TGroup>(in ObjectStore store, in SingeLinkGraph graph, in TGroup from, in TGroup to)
            where TGroup : notnull
        {
            var fromId = store.Save(from);
            var toId = store.Save(to);
            var status = graph.CreateLink(fromId, toId);
            if (status == false)
            {
                Log.Warn($"Ignored duplicate. Link({from}, {to})");
            }
        }

        public static List<int> GatherIdListWithErrorHandle(in SingeLinkGraph graph, in string sourceName)
        {
            List<int> ids;
            try
            {
                ids = graph.TopologicalOrderList();
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"Failed to gather items from {sourceName}-source.", e);
            }
            return ids;
        }
    }
}