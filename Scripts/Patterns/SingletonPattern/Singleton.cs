using UnityEngine;

namespace VT.Patterns.SingletonPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private bool persistent;

        private static T _instance;

        private static bool _isShuttingDown = false;

        // Lock object to prevent race conditions for thread safety
        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    Debug.LogWarning($"[{typeof(T).Name}] Instance is shutting down. Returning null.");
                    return null;
                }

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

        protected virtual void OnDestroy()
        {
            // Mark that we’re in teardown, so Instance will return null from now on
            _isShuttingDown = true;

            // If this was our instance, clear the reference
            if (_instance == this)
                _instance = null;
        }
    }
}
