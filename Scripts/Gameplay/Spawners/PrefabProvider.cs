using System;
using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.ObjectPoolPattern;

namespace VT.Gameplay.Spawners
{
    /// <summary>
    /// Abstract base for spawning pooled prefabs according to a selection strategy.
    /// </summary>
    public abstract class PrefabProvider
    {
        /// <summary>
        /// List of prefab GameObjects that have a PooledObject component.
        /// </summary>
        public List<GameObject> Prefabs = new();

        public event Action<GameObject> OnGet = _ => { };
        public event Action<GameObject> OnReturned = _ => { };

        /// <summary>
        /// Initializes pools for each prefab. Call once (e.g., in Awake or OnEnable).
        /// </summary>
        public void Initialize(int preloadEach = 0)
        {
            if (isInitialized) return;

            foreach (var go in Prefabs)
            {
                if (go.TryGetComponent<PooledObject>(out var pooledObject))
                {
                    // Create or fetch the pool and optionally preload
                    ObjectPool<PooledObject> pool = ObjectPoolManager.Instance.GetOrCreatePool(pooledObject);
                    
                    pool.OnGet += (item) => OnGet(item.GetGameObject());
                    pool.OnReturned += (item) => OnReturned(item.GetGameObject());
                    
                    if (preloadEach > 0)
                        pool.Preload(preloadEach);
                }
            }

            isInitialized = true;
        }

        /// <summary>
        /// Spawns the next prefab based on the strategy.
        /// </summary>
        public GameObject GetPrefab()
        {
            var go = QueryNextPrefab();
            if (go != null && go.TryGetComponent<PooledObject>(out var pooledObject))
            {
                // Spawn via manager and return its GameObject
                var instance = ObjectPoolManager.Instance.Get(pooledObject);
                if (instance == null)
                {
                    Debug.LogWarning($"PrefabProvider: Failed to get instance for prefab {go.name}. Returning null.");
                    return null;
                }
                return instance.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Returns an instance back to its originating pool (or destroys if not tracked).
        /// </summary>
        public void ReturnPrefab(GameObject instance)
        {
            if (instance == null) return;

            if (instance.TryGetComponent<PooledObject>(out var pooledObject))
            {
                ObjectPoolManager.Instance.Return(pooledObject);
            }
            else
            {
                // Fallback: destroy if it's not a pooled object
                UnityEngine.Object.Destroy(instance);
            }
        }

        /// <summary>
        /// Strategy hook: pick which prefab to spawn next.
        /// </summary>
        public abstract GameObject QueryNextPrefab();

        protected bool isInitialized = false;
    }
}
