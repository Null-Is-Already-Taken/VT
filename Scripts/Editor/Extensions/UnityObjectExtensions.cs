using UnityEditor;
using UnityEngine;

namespace VT.Editor.Extensions
{
    public static class UnityObjectExtensions
    {
        public static void PingAndSelect(this Object obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Attempted to ping a null object.");
                return;
            }
            // Use UnityEditor.EditorGUIUtility.PingObject for pinging in the editor
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
    }
}
