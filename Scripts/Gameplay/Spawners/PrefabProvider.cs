using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.ObjectPoolPattern;
using VT.Patterns.ObjectPoolPattern.Extras;

namespace VT.Gameplay.Spawners
{
    public abstract class PrefabProvider
    {
        public List<GameObject> Prefabs = new();
        private Transform poolParent;

        /// <summary>
        /// Build pools for every prefab. Call this once (e.g., from Awake/OnEnable).
        /// </summary>
        public void InitPool(int preloadEach = 0, Transform poolParent = null)
        {
            this.poolParent = poolParent ?? new GameObject("PrefabProvider Pool").transform;

            foreach (var prefab in Prefabs)
            {
                if (prefab.TryGetComponent<PooledObject>(out var pooledObject))
                {
                    var pool = ObjectPoolManager.Instance.CreatePool(prefab.name, pooledObject, this.poolParent);
                    if (preloadEach > 0) pool.Preload(preloadEach);
                }
            }
        }

        /// <summary>
        /// Get a pooled instance of whichever prefab your strategy picks.
        /// </summary>
        public GameObject GetPrefab()
        {
            var prefab = QueryNextPrefab();
            if (!prefab) return null;

            var pool = ObjectPoolManager.Instance.GetPool<PooledObject>(prefab.name);

            if (pool == null)
            {
                Debug.LogWarning($"No pool for {prefab.name}. Creating on the fly.");
                if (prefab.TryGetComponent<PooledObject>(out var pooledObject))
                {
                    pool = ObjectPoolManager.Instance.CreatePool(prefab.name, pooledObject, poolParent);
                    pool.Preload(1);
                }
            }

            // Grab it
            var t = pool.Get();
            return t.gameObject;
        }

        /// <summary>
        /// Return an instance to its pool.
        /// </summary>
        public void ReturnPrefab(GameObject instance)
        {
            if (!instance) return;

            if (instance.TryGetComponent<PooledObject>(out var pooledObject))
            {
                var pool = ObjectPoolManager.Instance.GetPool<PooledObject>(pooledObject.SourcePrefab.name);
                if (pool != null)
                {
                    pool.Return(pooledObject);
                }
                else
                {
                    // Fallback: destroy if we can't find a pool
                    Object.Destroy(instance);
                }
            }
        }

        public abstract GameObject QueryNextPrefab();
    }
}
