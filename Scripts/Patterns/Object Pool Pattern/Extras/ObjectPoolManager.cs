using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.SingletonPattern;

namespace VT.Patterns.ObjectPoolPattern.Extras
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private readonly Dictionary<string, object> pools = new();

        public void AddPool<T>(string poolName, ObjectPool<T> pool) where T : Component
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogWarning("poolName [" + poolName + "] is null/empty. Using prefab name as pool name.");
                poolName = pool.Get().name + "Pool";
            }

            if (pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool with name {poolName} already exists. Overwriting.");
            }

            pools[poolName] = pool;
        }

        public ObjectPool<T> GetPool<T>(string poolName) where T : Component
        {
            if (pools.TryGetValue(poolName, out var poolObj) && poolObj is ObjectPool<T> pool)
            {
                return pool;
            }

            Debug.LogWarning($"Pool with name {poolName} does not exist.");
            return null;
        }

        public ObjectPool<T> CreatePool<T>(string poolName, T prefab, Transform parent = null) where T : Component
        {
            ObjectPool<T> pool = new(prefab, parent);
            AddPool(poolName, pool);
            return pool;
        }
    }
}