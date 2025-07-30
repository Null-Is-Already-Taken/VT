using UnityEngine;

namespace VT.Patterns.SingletonPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private bool persistent;

        private static T _instance;

        // Lock object to prevent race conditions for thread safety
        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = FindAnyObjectByType<T>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new(typeof(T).Name);
                            _instance = singletonObject.AddComponent<T>();
                            (_instance as Singleton<T>)?.LazyInit();
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                (_instance as Singleton<T>)?.LazyInit();

                if (persistent)
                {
                    DontDestroyOnLoad(gameObject); // Optional, remove if you don’t want it to persist between scenes
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject); // Destroy duplicate singleton instances
            }
        }

        protected virtual void LazyInit()
        {
        }
    }
}
