using UnityEngine;

namespace VT.Patterns.ObjectPoolPattern
{
    public class PooledObject : MonoBehaviour, IPoolable
    {
        public GameObject SourcePrefab;

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