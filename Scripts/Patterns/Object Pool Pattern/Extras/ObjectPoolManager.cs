using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.SingletonPattern;

namespace VT.Patterns.ObjectPoolPattern.Extras
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

            if (poolsByPrefab.TryGetValue(prefab, out var existing))
            {
                return (ObjectPool<T>)existing;
            }

            var pool = ObjectPool<T>.Create(prefab);
            poolsByPrefab[prefab] = pool;

            // Track instances as they are fetched/returned
            pool.OnGet += item => instanceToPool[item] = pool;
            pool.OnReturned += item => instanceToPool.Remove(item);

            return pool;
        }

        /// <summary>
        /// Convenience method to spawn an instance from the pool.
        /// </summary>
        public T Spawn<T>(T prefab) where T : MonoBehaviour, IPooledObject
        {
            var pool = GetOrCreatePool(prefab);
            return (T)pool.Get();
        }

        /// <summary>
        /// Release a spawned instance back into its pool.
        /// If no pool tracks this instance, it will be destroyed.
        /// </summary>
        public void Release(IPooledObject instance)
        {
            if (instance == null) return;

            if (instanceToPool.TryGetValue(instance, out var pool))
            {
                pool.Return(instance);
            }
            else
            {
                Debug.LogWarning($"ObjectPoolManager: instance '{instance.Name}' not tracked. Destroying.");
                UnityEngine.Object.Destroy(instance.GameObject);
            }
        }

        /// <summary>
        /// Clears and disposes all pools managed by this manager.
        /// </summary>
        public void ClearAll()
        {
            foreach (var kv in poolsByPrefab)
            {
                kv.Value.Dispose();
            }
            poolsByPrefab.Clear();
            instanceToPool.Clear();
        }
    }
}
