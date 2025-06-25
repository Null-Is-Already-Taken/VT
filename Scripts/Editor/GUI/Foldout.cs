using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using VT.Editor.Utils;

namespace VT.Editor.GUI
{
    public static class Foldout
    {
        public static void Draw(
            string title,
            string tooltip,
            string key,
            Dictionary<string, AnimBool> foldoutStateCache,
            UnityAction repaintCallback,
            Func<string> subScriptGetter,
            Action drawContentCallback,
            bool defaultFoldoutState = true)
        {
            if (!foldoutStateCache.TryGetValue(key, out var foldState))
            {
                foldState = new AnimBool(defaultFoldoutState);
                foldState.valueChanged.AddListener(repaintCallback);
                foldoutStateCache[key] = foldState;
            }

            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    string icon = foldState.target ? EmbeddedIcons.TriangleBlackDownSmall_Unicode : EmbeddedIcons.TriangleBlackRightSmall_Unicode;

                    Button.Draw(
                        content: new GUIContent(text: $"{icon} {title}", tooltip: tooltip),
                        backgroundColor: Color.white,
                        onClick: () => foldState.target = !foldState.target,
                        style: EditorStyles.boldLabel
                    );

                    GUILayout.FlexibleSpace();

                    // Draw subscript if provided
                    if (subScriptGetter != null)
                    {
                        Label.DrawAutoSized(
                            text: subScriptGetter.Invoke(),
                            style: LabelStyles.MiniLabel
                        );
                    }
                }

                using var fg = new EditorGUILayout.FadeGroupScope(foldState.faded);
                if (fg.visible)
                {
                    drawContentCallback?.Invoke();
                }
            }
        }
    }
}
