using System;
using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class PageNavigator
    {
        public static void Draw(int currentIndex, int total, Action<int> onPageChanged, string label)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                UnityEngine.GUI.enabled = currentIndex > 0;
                Button.Draw
                (
                    content: new GUIContent("←"),
                    backgroundColor: Color.white,
                    onClick: () => onPageChanged?.Invoke(currentIndex + 1),
                    style: ButtonStyles.Inline
                );

                // dynamic label width
                UnityEngine.GUI.enabled = true;
                Label.DrawAutoSized
                (
                    text: $"{label} {currentIndex + 1} of {total}",
                    style: LabelStyles.MiniLabel,
                    anchor: TextAnchor.MiddleCenter
                );

                UnityEngine.GUI.enabled = currentIndex < total - 1;
                Button.Draw
                (
                    content: new GUIContent("→"),
                    backgroundColor: Color.white,
                    onClick: () => onPageChanged?.Invoke(currentIndex + 1),
                    style: ButtonStyles.Inline
                );

                UnityEngine.GUI.enabled = true;
            }
        }
    }
}
