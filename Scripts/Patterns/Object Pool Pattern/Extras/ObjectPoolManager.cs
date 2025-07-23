using System.Collections.Generic;
using UnityEngine;
using VT.Patterns.SingletonPattern;

namespace VT.Patterns.ObjectPoolPattern.Extras
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private readonly Dictionary<string, object> pools = new();

        public void AddPool<T>(ObjectPool<T> pool) where T : PooledObject
        {
            if (pools.ContainsKey(pool.PrefabName))
            {
                Debug.LogWarning($"Pool with name {pool.PrefabName} already exists. Overwriting.");
            }

            pools[pool.PrefabName] = pool;
        }

        public ObjectPool<T> GetPool<T>(T prefab) where T : PooledObject
        {
            string poolName = GeneratePoolName(prefab);

            if (pools.TryGetValue(poolName, out var poolObj) && poolObj is ObjectPool<T> pool)
            {
                return pool;
            }

            Debug.LogWarning($"Pool with name {poolName} does not exist.");
            return null;
        }

        public ObjectPool<T> CreatePool<T>(T prefab, Transform parent = null) where T : PooledObject
        {
            string poolName = GeneratePoolName(prefab);

            if (parent == null)
            {
                parent = new GameObject(poolName).transform;
            }

            ObjectPool<T> pool = ObjectPool<T>.Create(prefab, parent);
            AddPool(pool);
            return pool;
        }

        public static string GeneratePoolName(PooledObject prefab)
        {
            return GeneratePoolName(prefab.name);
        }

        public static string GeneratePoolName(string prefabName)
        {
            return prefabName + " Pool";
        }
    }
}