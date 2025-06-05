using UnityEngine;

namespace VT.Utils
{
    public static class LazyGetter
    {
        /// <summary>
        /// Lazily initialize hook.
        /// </summary>
        public static T InitHook<T>(ref T hook, System.Func<T> constructor) where T : class
        {
            if (hook == null)
            {
                hook = constructor();
            }

            return hook;
        }

        /// <summary>
        /// Lazy loads a component of type T on the given GameObject if it's not already assigned.
        /// </summary>
        /// <typeparam name="T">Type of component to get or add if missing.</typeparam>
        /// <param name="transform">The transform to retrieve the component from.</param>
        /// <param name="cachedComponent">The cached component reference.</param>
        /// <returns>The component of type T.</returns>
        public static T GetComponent<T>(Transform transform, ref T cachedComponent) where T : Component
        {
            if (cachedComponent == null)
            {
                cachedComponent = transform.GetComponent<T>();
                Debug.Assert(cachedComponent != null, $"There is no {typeof(T).Name} on {transform.name}.");
            }

            return cachedComponent;
        }

        /// <summary>
        /// Lazy loads a child transform with the specified name from the given parent transform.
        /// </summary>
        /// <param name="parentTransform">The parent transform to retrieve the child from.</param>
        /// <param name="childName">The name of the child transform to find.</param>
        /// <param name="cachedTransform">The cached transform reference.</param>
        /// <returns>The child transform with the specified name.</returns>
        public static Transform GetChildTransform(Transform parentTransform, string childName, ref Transform cachedTransform)
        {
            if (cachedTransform == null || cachedTransform.name != childName)
            {
                cachedTransform = parentTransform.Find(childName);
                Debug.Assert(cachedTransform != null, $"{childName} transform not found on {parentTransform.name}.");
            }

            return cachedTransform;
        }

        public static T FindComponent<T>(ref T cachedComponent) where T : Component
        {
            if (cachedComponent == null)
            {
                cachedComponent = Object.FindAnyObjectByType<T>();
                Debug.Assert(cachedComponent != null, $"Cannot find {nameof(T)} component in active scene.");
            }

            return cachedComponent;
        }
    }
}
