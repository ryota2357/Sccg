using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sccg.Utility;

/// <summary>
/// A simple object store.
/// </summary>
public class ObjectStore
{
    private readonly SortedDictionary<string, int> _idStore = new();
    private readonly List<object> _objStore = new();
    private readonly List<Type> _typeStore = new();

    /// <summary>
    /// Get the number of objects in store.
    /// </summary>
    public int Count { get; private set; } = 0;

    /// <summary>
    /// Save data to store then return the id.
    /// </summary>
    public int Save(in object obj)
    {
        var objId = obj.GetType() + obj.ToString() + obj.GetHashCode();
        var objType = obj.GetType();
        return Save(obj, objId, objType);
    }

    /// <summary>
    /// Load data from store with specified id.
    /// </summary>
    public (object data, Type type) Load(int id)
    {
        if ((uint)id >= _objStore.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }
        return (_objStore[id], _typeStore[id]);
    }

    /// <summary>
    /// Load data from store with specified id.
    /// </summary>
    public T Load<T>(int id)
    {
        var (data, type) = Load(id);
        if (data is T t)
        {
            return t;
        }
        else
        {
            throw new InvalidCastException($"Cannot cast {type} to {typeof(T)}");
        }
    }

    /// <summary>
    /// Try to load data from store with specified id.
    /// </summary>
    public bool TryLoad<T>(int id, [NotNullWhen(true)] out T? data)
    {
        if ((uint)id >= _objStore.Count)
        {
            data = default;
            return false;
        }

        var (obj, _) = Load(id);
        if (obj is T t)
        {
            data = t;
            return true;
        }
        else
        {
            data = default;
            return false;
        }
    }

    private int Save(object obj, string objId, Type type)
    {
        if (_idStore.TryGetValue(objId, out var value))
        {
            return value;
        }

        _idStore.Add(objId, Count);
        _objStore.Add(obj);
        _typeStore.Add(type);
        Count += 1;
        return Count - 1;
    }
}