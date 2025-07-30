using System;
using System.Collections.Generic;
using UnityEngine;
using VT.Gameplay.Spawners;
using VT.Patterns.ObjectPoolPattern;

public class SpawnRegistrar
{
    // we store the wrapped handlers per pool so we can detach them later
    private readonly Dictionary<IObjectPool, Action<IPooledObject>> _wrapped = new();

    public void Attach(PrefabProvider provider, Action<GameObject> handler)
    {
        foreach (var go in provider.Prefabs)
        {
            if (!go.TryGetComponent<PooledObject>(out var po)) continue;
            var pool = ObjectPoolManager.Instance.GetOrCreatePool(po);

            // wrap the IPooledObject callback in a GameObject callback
            void wrap(IPooledObject item) => handler(item.GetGameObject());

            pool.OnGet += wrap;
            _wrapped[pool] = wrap;
        }
    }

    public void Detach(PrefabProvider provider, Action<GameObject> handler)
    {
        foreach (var go in provider.Prefabs)
        {
            if (!go.TryGetComponent<PooledObject>(out var po)) continue;
            var pool = ObjectPoolManager.Instance.GetPool(po);
            if (pool != null && _wrapped.TryGetValue(pool, out var wrap))
            {
                pool.OnGet -= wrap;
                _wrapped.Remove(pool);
            }
        }
    }
}

public class DespawnRegistrar
{
    private readonly Dictionary<IObjectPool, Action<IPooledObject>> _wrapped = new();

    public void Attach(PrefabProvider provider, Action<GameObject> handler)
    {
        foreach (var go in provider.Prefabs)
        {
            if (!go.TryGetComponent<PooledObject>(out var po)) continue;
            var pool = ObjectPoolManager.Instance.GetOrCreatePool(po);

            void wrap(IPooledObject item) => handler(item.GetGameObject());

            pool.OnReturned += wrap;
            _wrapped[pool] = wrap;
        }
    }

    public void Detach(PrefabProvider provider, Action<GameObject> handler)
    {
        foreach (var go in provider.Prefabs)
        {
            if (!go.TryGetComponent<PooledObject>(out var po)) continue;
            var pool = ObjectPoolManager.Instance.GetPool(po);
            if (pool != null && _wrapped.TryGetValue(pool, out var wrap))
            {
                pool.OnReturned -= wrap;
                _wrapped.Remove(pool);
            }
        }
    }
}
