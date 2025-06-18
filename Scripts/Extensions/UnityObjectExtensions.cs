using UnityEngine;

namespace VT.Extensions
{
    public static class UnityObjectExtensions
    {
#if UNITY_EDITOR
        public static void PingAndSelect(this Object obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Attempted to ping a null object.");
                return;
            }
            // Use UnityEditor.EditorGUIUtility.PingObject for pinging in the editor
            UnityEditor.EditorGUIUtility.PingObject(obj);
            
        }
#endif
    }
}
