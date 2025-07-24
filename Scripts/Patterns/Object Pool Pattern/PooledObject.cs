using UnityEngine;

namespace VT.Patterns.ObjectPoolPattern
{
    public abstract class PooledObject : MonoBehaviour, IPooledObject
    {
        public GameObject SourcePrefab => gameObject;

        public string Name => name;

        public GameObject GameObject => gameObject;

        public virtual void OnReturnedToPool()
        {
            Debug.Log($"Returning {gameObject.name} to pool.");
        }

        public virtual void OnSpawned()
        {
            Debug.Log($"Spawned {gameObject.name} from pool.");
        }
    }
}