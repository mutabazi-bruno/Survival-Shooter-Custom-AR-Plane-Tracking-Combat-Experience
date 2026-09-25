using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// Generic object pool. Every object is created once up front (when the pool is built),
// then just switched on and off. Nothing is Instantiated or Destroyed while playing.
public class ObjectPool<T> where T : Component
{
    readonly Queue<T> available = new();
    readonly List<T> inUse = new();

    public int Size { get; }
    public int CountInUse => inUse.Count;

    public ObjectPool(T prefab, int size, Transform container, Action<T> onCreated = null)
    {
        Size = size;

        for (int i = 0; i < size; i++)
        {
            T item = Object.Instantiate(prefab, container);
            item.name = $"{prefab.name} {i}";
            item.gameObject.SetActive(false);
            onCreated?.Invoke(item);
            available.Enqueue(item);
        }
    }

    public T Get()
    {
        // if everything is already out, recycle the oldest one instead of creating a new object mid-game
        T item = available.Count > 0 ? available.Dequeue() : TakeBackOldest();

        inUse.Add(item);
        item.gameObject.SetActive(true);
        if (item is IPoolable poolable) poolable.OnTakenFromPool();
        return item;
    }

    public void Release(T item)
    {
        // already back in the pool (e.g. hit something on the same frame its lifetime ran out)
        if (!inUse.Remove(item)) return;

        if (item is IPoolable poolable) poolable.OnReturnedToPool();
        item.gameObject.SetActive(false);
        available.Enqueue(item);
    }

    public void ReleaseAll()
    {
        for (int i = inUse.Count - 1; i >= 0; i--)
            Release(inUse[i]);
    }

    T TakeBackOldest()
    {
        T oldest = inUse[0];
        Release(oldest);
        return available.Dequeue();
    }
}
