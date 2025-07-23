using System.Collections.Generic;
using UnityEngine;

namespace VT.Patterns.ObjectPoolPattern
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnReturnedToPool();
    }

    public class ObjectPool<T> where T : PooledObject
    {
        public string PrefabName => prefab.name;

        public static ObjectPool<T> Create(T prefab, Transform parent = null)
        {
            if (prefab == null) return null;
            ObjectPool<T> pool = new()
            {
                prefab = prefab,
                parent = parent
            };
            return pool;
        }

        public int Count => pool.Count;

        public T Get()
        {
            T item;

            if (pool.Count > 0)
            {
                item = pool.Pop();
            }
            else
            {
                item = Object.Instantiate(prefab, parent);
            }

            Activate(item, active: true);
            return item;
        }

        public bool TryGet(out T item)
        {
            if (pool.Count > 0)
            {
                item = pool.Pop();
                Activate(item, active: true);
                return true;
            }
            item = null;
            return false;
        }

        public void Return(T item)
        {
            if (item == null) return;

            Activate(item, active: false);
            pool.Push(item);
        }

        public void Clear()
        {
            while (pool.Count > 0)
            {
                Object.Destroy(pool.Pop().gameObject);
            }
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T item = Object.Instantiate(prefab, parent);
                Return(item);
            }
        }

        /// <summary>
        /// Override this method to handle additional logic when an object is retrieved from the pool
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnObjectGet(T item)
        {
        }

        private void Activate(T item, bool active)
        {
            if (item == null || item.gameObject == null)
            {
                poolableCache.Remove(item);
                return;
            }

            item.transform.SetParent(active ? null : parent, active);
            item.gameObject.SetActive(active);

            if (!poolableCache.TryGetValue(item, out var poolable))
            {
                poolable = item.GetComponent<IPoolable>();

                if (poolable != null)
                {
                    poolableCache[item] = poolable;
                }
            }

            if (active)
            {
                poolable?.OnSpawned();
            }
            else
            {
                poolable?.OnReturnedToPool();
            }
        }

        protected T prefab;
        protected Stack<T> pool = new();
        protected Transform parent;
        protected Dictionary<T, IPoolable> poolableCache = new();
    }
}