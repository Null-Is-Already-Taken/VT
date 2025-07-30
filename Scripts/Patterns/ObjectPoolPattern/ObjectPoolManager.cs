using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.SingletonPattern;

namespace VT.Patterns.ObjectPoolPattern
{
    /// <summary>
    /// Centralized manager for all object pools. Handles creation, instance tracking, spawn & release.
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        // Map from prefab asset -> its pool
        private readonly Dictionary<IPooledObject, IObjectPool> poolsByPrefab = new();
        // Map each spawned instance -> its originating pool
        private readonly Dictionary<IPooledObject, IObjectPool> instanceToPool = new();

        public ObjectPool<T> GetPool<T>(T prefab) where T : MonoBehaviour, IPooledObject
        {
            if (prefab == null)
            {
                Debug.LogWarning("ObjectPoolManager: prefab is null. Cannot get pool.");
                return null;
            }

            if (poolsByPrefab.TryGetValue(prefab, out var existing))
            {
                return (ObjectPool<T>)existing;
            }
            Debug.LogWarning($"ObjectPoolManager: No pool found for prefab {prefab.name}. Returning null.");
            return null;
        }

        /// <summary>
        /// Get or create a pool for the given prefab.
        /// </summary>
        public ObjectPool<T> GetOrCreatePool<T>(T prefab) where T : MonoBehaviour, IPooledObject
        {
            if (prefab == null)
            {
                Debug.LogWarning("ObjectPoolManager: prefab is null. Cannot create pool.");
                return null;
            }

            var pool = GetPool(prefab);

            if (pool != null)
            {
                // Pool already exists, return it
                return pool;
            }

            pool = ObjectPool<T>.Create(prefab, container: transform);
            poolsByPrefab[prefab] = pool;

            // Track instances as they are fetched/returned
            pool.OnGet += item => instanceToPool[item] = pool;
            pool.OnReturned += item => instanceToPool.Remove(item);

            return pool;
        }

        /// <summary>
        /// Convenience method to spawn an instance from the pool.
        /// </summary>
        public T Get<T>(T prefab) where T : MonoBehaviour, IPooledObject
        {
            var pool = GetOrCreatePool(prefab);
            Debug.Log("ObjectPoolManager: Getting instance from pool for prefab " + prefab.name);
            if (pool == null)
            {
                Debug.LogWarning($"ObjectPoolManager: No pool found for prefab {prefab.name}. Returning null.");
                return null;
            }
            return (T)pool.Get();
        }

        /// <summary>
        /// Release a spawned instance back into its pool.
        /// If no pool tracks this instance, it will be destroyed.
        /// </summary>
        public void Return(IPooledObject instance)
        {
            if (instance == null) return;

            // 1) Log every call and whether we think it’s tracked
            bool tracked = instanceToPool.ContainsKey(instance);

            if (tracked && instanceToPool.TryGetValue(instance, out var pool))
            {
                // 2) We found its pool → return it
                pool.Return(instance);
            }
            else
            {
                // 3) No mapping found → fallback
                Destroy(instance.GetGameObject());
            }
        }

        public void Return(GameObject go)
        {
            if (go == null) return;
            if (go.TryGetComponent<IPooledObject>(out var pooled))
                Return(pooled);
            else
            {
                Destroy(go);
            }
        }

        /// <summary>
        /// Clears and disposes all pools managed by this manager.
        /// </summary>
        public void CleanUp()
        {
            foreach (var kv in poolsByPrefab)
            {
                kv.Value.Dispose();
            }
            poolsByPrefab.Clear();
            instanceToPool.Clear();
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}
