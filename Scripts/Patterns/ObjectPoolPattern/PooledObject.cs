using UnityEngine;

namespace VT.Patterns.ObjectPoolPattern
{
    public abstract class PooledObject : MonoBehaviour, IPooledObject
    {
        public string Name => name;

        public GameObject GameObject => gameObject ? gameObject : null;

        public virtual void OnSpawned()
        {
        }

        public virtual void OnReturned()
        {
        }

        /// <summary>
        /// Call this whenever you want to return yourself to the pool.
        /// </summary>
        protected void ReturnToPool()
        {
            ObjectPoolManager.Instance.Return(this);
        }
    }
}