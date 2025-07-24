using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.ObjectPoolPattern;
using VT.Patterns.ObjectPoolPattern.Extras;

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

        /// <summary>
        /// Initializes pools for each prefab. Call once (e.g., in Awake or OnEnable).
        /// </summary>
        public void Initialize(int preloadEach = 0)
        {
            foreach (var go in Prefabs)
            {
                if (go.TryGetComponent<PooledObject>(out var pooledObject))
                {
                    // Create or fetch the pool and optionally preload
                    var pool = ObjectPoolManager.Instance.GetOrCreatePool(pooledObject);
                    if (preloadEach > 0)
                        pool.Preload(preloadEach);
                }
                else
                {
                    Debug.LogWarning($"PrefabProvider: {go.name} missing PooledObject component.");
                }
            }
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
                var instance = ObjectPoolManager.Instance.Spawn(pooledObject);
                return instance.GameObject;
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
                ObjectPoolManager.Instance.Release(pooledObject);
            }
            else
            {
                // Fallback: destroy if it's not a pooled object
                Object.Destroy(instance);
            }
        }

        /// <summary>
        /// Strategy hook: pick which prefab to spawn next.
        /// </summary>
        public abstract GameObject QueryNextPrefab();
    }
}
