#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

namespace VT.Tools.EssentialAssetsImporter
{
    public class GitUrlInputPopup : EditorWindow
    {
        private string gitUrl = "";
        private Action<string> onSubmit;

        public static void Show(Action<string> onSubmitCallback)
        {
            GitUrlInputPopup window = ScriptableObject.CreateInstance<GitUrlInputPopup>();
            window.titleContent = new GUIContent("Enter Git URL");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 80);
            window.onSubmit = onSubmitCallback;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Git URL (https:// or git@):", EditorStyles.boldLabel);
            gitUrl = EditorGUILayout.TextField(gitUrl);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                onSubmit?.Invoke(gitUrl);
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
