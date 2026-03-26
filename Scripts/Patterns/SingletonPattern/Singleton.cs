using UnityEngine;

namespace VT.Patterns.SingletonPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool persistent;

        private static T _instance;
        private static bool _isShuttingDown;
        private static readonly object _lock = new();

        private static bool _isCreatingFromInstance;
        private bool _didInit;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    Debug.LogWarning($"[{typeof(T).Name}] Instance is shutting down. Returning null.");
                    return null;
                }

                if (_instance != null)
                    return _instance;

                lock (_lock)
                {
                    if (_instance != null)
                        return _instance;

                    // IMPORTANT: include inactive so we don’t accidentally create a new one
#if UNITY_2023_1_OR_NEWER
                    _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
                    _instance = FindObjectOfType<T>(true);
#endif

                    if (_instance != null)
                    {
                        if (_instance is Singleton<T> found)
                            found.EnsureInitialized();

                        return _instance;
                    }

                    // Create
                    var go = new GameObject(typeof(T).Name);

                    _isCreatingFromInstance = true;
                    _instance = go.AddComponent<T>(); // Awake runs here
                    _isCreatingFromInstance = false;

                    if (_instance is Singleton<T> created)
                        created.EnsureInitialized();

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                // If created normally (scene), init now
                if (!_isCreatingFromInstance)
                    EnsureInitialized();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                // ✅ Key: if Instance assigned us before our Awake ran, still init here too
                EnsureInitialized();
            }
        }

        private void EnsureInitialized()
        {
            if (_didInit)
                return;

            _didInit = true;

            LazyInit();

            if (persistent)
            {
                if (transform.parent != null)
                    transform.SetParent(null);

                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void LazyInit() { }

        protected virtual void OnDestroy()
        {
            // Only shutdown if the actual singleton instance is being destroyed
            if (_instance == this)
            {
                _isShuttingDown = true;
                _instance = null;
            }
        }
    }
}
