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
            string key,
            Dictionary<string, AnimBool> foldoutStateCache,
            UnityAction repaintCallback,
            Func<int> getItemCountFunc,
            Action drawContentCallback,
            bool defaultFoldoutState = true)
        {
            if (!foldoutStateCache.TryGetValue(key, out var foldState))
            {
                foldState = new AnimBool(defaultFoldoutState);
                foldState.valueChanged.AddListener(repaintCallback);
                foldoutStateCache[key] = foldState;
            }

            int itemCount = getItemCountFunc?.Invoke() ?? 0;

            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var icon = foldState.target ? EmbeddedIcons.TriangleBlackDown_Unicode : EmbeddedIcons.TriangleBlackRight_Unicode;
                    Button.Draw(
                        content: new GUIContent(icon),
                        backgroundColor: Color.white,
                        onClick: () => foldState.target = !foldState.target,
                        style: ButtonStyles.MiniInline
                    );

                    Label.DrawTruncatedLabel(
                        fullText: title,
                        textColor: Color.white,
                        tooltip: title,
                        availableWidth: EditorGUIUtility.currentViewWidth,
                        averageCharWidth: 6f,
                        style: LabelStyles.BoldLabel
                    );

                    GUILayout.FlexibleSpace();

                    if (itemCount > 0)
                    {
                        // Draw item count if applicable
                        Label.Draw(
                            text: $"({itemCount})",
                            style: LabelStyles.MiniLabel,
                            options: GUILayout.Width(EditorGUIUtility.singleLineHeight)
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
