using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sccg.Utility;

public class SingeLinkGraph
{
    private readonly List<int?> _adjacentList;

    public SingeLinkGraph() : this(capacity: 0)
    {
    }

    public SingeLinkGraph(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _adjacentList = new List<int?>(capacity);
    }

    public bool CreateLink(int from, int to, bool overwrite = false)
    {
        if (from < 0 || to < 0)
        {
            throw new ArgumentException("`from` and `to` must be greater than 0");
        }

        var max = Math.Max(from, to);
        var has = _adjacentList.Count > from && _adjacentList[from] != null;

        if (has && !overwrite)
        {
            return false;
        }

        if (max >= _adjacentList.Count)
        {
            _adjacentList.AddRange(Enumerable.Repeat((int?)null, max - _adjacentList.Count + 1));
        }

        _adjacentList[from] = to;
        return true;
    }

    public int? GetLink(int from)
    {
        if (from < 0)
        {
            throw new ArgumentException("`from` must be greater than 0");
        }
        return _adjacentList.Count <= from ? null : _adjacentList[from];
    }

    public bool TryGetLink(int from, [NotNullWhen(true)] out int? to)
    {
        to = GetLink(from);
        return to != null;
    }

    /// <summary>
    /// Get an array of all vertex numbers in topological order. <br/>
    /// Note: This method throws <see cref="InvalidOperationException"/> if the graph is not a DAG.
    /// </summary>
    /// <returns>
    ///   Topological sorted list.
    ///   <code>
    ///    [0]  [1] ← [2]  [4]  [5]
    ///                  ↖    ↙
    ///                   [3] ← [6]
    ///   </code>
    /// </returns>
    /// <exception cref="InvalidOperationException">Cycle is detected</exception>
    public List<int> TopologicalOrderList()
    {
        var result = new List<int>();
        var visited = new bool[_adjacentList.Count];
        var stack = new Stack<int>();

        for (var i = 0; i < _adjacentList.Count; i++)
        {
            if (visited[i])
            {
                continue;
            }
            stack.Push(i);
            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var next = GetLink(current);
                if (next == null)
                {
                    result.Add(current);
                    visited[current] = true;
                    stack.Pop();
                    continue;
                }
                if (visited[next.Value])
                {
                    result.Add(current);
                    visited[current] = true;
                    stack.Pop();
                    continue;
                }
                if (stack.Contains(next.Value))
                {
                    throw new InvalidOperationException("Cycle detected");
                }
                stack.Push(next.Value);
            }
        }

        return result;
    }
}