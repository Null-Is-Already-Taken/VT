using System;
using System.Collections.Generic;
using UnityEngine;

namespace VT.Patterns.ObjectPoolPattern
{
    #region Interfaces

    public interface IPooledObject
    {
        string Name { get; }
        void OnSpawned();
        void OnReturnedToPool();
    }

    public interface IObjectPool : IDisposable
    {
        IPooledObject Prefab { get; }
        Transform Parent { get; }
        public int Count { get; }

        event Action<IPooledObject> OnGet;
        event Action<IPooledObject> OnReturned;

        T GetCasted<T>() where T : MonoBehaviour, IPooledObject;
        IPooledObject Get();
        void Return(IPooledObject item);
        bool TryGet(out IPooledObject item);
        void Preload(int count);
        void Clear();
    }

    #endregion

    public class ObjectPool<T> : IObjectPool where T : MonoBehaviour, IPooledObject
    {
        #region Factory

        public static ObjectPool<T> Create(T prefab, Transform container = null)
        {
            if (prefab == null) return null;
            return new(prefab, container);
        }

        #endregion

        public event Action<IPooledObject> OnGet = _ => { };
        public event Action<IPooledObject> OnReturned = _ => { };

        #region IObjectPool Implementation

        public IPooledObject Prefab => prefab;
        public Transform Parent => objectContainer;
        public int Count => pool.Count;

        public U GetCasted<U>() where U : MonoBehaviour, IPooledObject => (U)Get();

        public IPooledObject Get()
        {
            IPooledObject item;
            if (pool.Count > 0)
            {
                item = pool.Pop();
            }
            else
            {
                var go = UnityEngine.Object.Instantiate(prefab, objectContainer);
                go.name = prefab.Name;
                item = go.GetComponent<T>();
            }

            Activate(item);
            OnGet?.Invoke(item);
            return item;
        }

        public void Return(IPooledObject item)
        {
            if (item == null) return;
            Deactivate(item);
            OnReturned?.Invoke(item);
            pool.Push(item);
        }

        public bool TryGet(out IPooledObject item)
        {
            if (pool.Count > 0)
            {
                item = pool.Pop();
                Activate(item);
                OnGet?.Invoke(item);
                return true;
            }
            item = null;
            return false;
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T item = UnityEngine.Object.Instantiate(prefab, objectContainer);
                item.name = prefab.name;
                Deactivate(item);
                pool.Push(item);
            }
        }

        public void Clear()
        {
            while (pool.Count > 0)
            {
                var entry = pool.Pop();
                if (entry == null)
                    continue;

                var go = entry.GetGameObject();
                if (go != null)
                    UnityEngine.Object.Destroy(go);
            }
        }

        public void ClearAndDestroy()
        {
            Clear();

            if (objectContainer)
            {
                UnityEngine.Object.Destroy(objectContainer.gameObject);
                objectContainer = null;
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                ClearAndDestroy();
                OnGet = _ => { };
                OnReturned = _ => { };
                isDisposed = true;
            }
        }

        ~ObjectPool() => Dispose(false);

        #endregion

        #region Protected

        protected T prefab;
        protected Stack<IPooledObject> pool = new();
        protected Transform objectContainer;
        protected bool isDisposed = false;

        protected ObjectPool(T prefab, Transform container = null)
        {
            this.prefab = prefab;
            objectContainer = new GameObject($"{prefab.name} Pool").transform;
            objectContainer.SetParent(container, false);
        }

        protected T CreateNew()
        {
            var go = UnityEngine.Object.Instantiate(prefab, objectContainer);
            go.name = prefab.Name;
            return go;
        }

        protected void Activate(IPooledObject item)
        {
            if (item == null) return;
            item.GetGameObject().transform.SetParent(null, true);
            item.GetGameObject().SetActive(true);
            item.OnSpawned();
        }

        protected void Deactivate(IPooledObject item)
        {
            if (item == null) return;
            item.GetGameObject().transform.SetParent(objectContainer, true);
            item.GetGameObject().SetActive(false);
            item.OnReturnedToPool();
        }

        #endregion

        #region Helper Methods

        public override string ToString()
        {
            return $"{prefab.Name} Pool ({pool.Count} items)";
        }

        #endregion
    }

    public static class IPooledObjectExtensions
    {
        public static GameObject GetGameObject(this IPooledObject pooledObject)
        {
            if (pooledObject is MonoBehaviour mb)
            {
                return mb.gameObject;
            }
            return null;
        }
    }
}