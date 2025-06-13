#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VT.Editor.GUI
{
    public static class LabelStyles
    {
        public static GUIStyle Label => new (EditorStyles.label);
        public static GUIStyle MiniLabel => new (EditorStyles.miniLabel);
        public static GUIStyle LargeLabel => new (EditorStyles.largeLabel);
        public static GUIStyle BoldLabel => new (EditorStyles.boldLabel);
        public static GUIStyle MiniBoldLabel => new (EditorStyles.miniBoldLabel);
        public static GUIStyle CenteredGreyMiniLabel => new (EditorStyles.centeredGreyMiniLabel);
        public static GUIStyle WordWrappedMiniLabel => new (EditorStyles.wordWrappedMiniLabel);
        public static GUIStyle WordWrappedLabel => new (EditorStyles.wordWrappedLabel);
        public static GUIStyle LinkLabel => new (EditorStyles.linkLabel);
        public static GUIStyle WhiteLabel => new (EditorStyles.whiteLabel);
        public static GUIStyle WhiteMiniLabel => new (EditorStyles.whiteMiniLabel);
        public static GUIStyle WhiteLargeLabel => new (EditorStyles.whiteLargeLabel);
        public static GUIStyle WhiteBoldLabel => new (EditorStyles.whiteBoldLabel);

        //public static GUIStyle TextField => new (EditorStyles.textField);
        //public static GUIStyle TextArea => new (EditorStyles.textArea);
        //public static GUIStyle MiniTextField => new (EditorStyles.miniTextField);
        //public static GUIStyle NumberField => new (EditorStyles.numberField);
        //public static GUIStyle Popup => new (EditorStyles.popup);

        //public static GUIStyle ObjectField => new (EditorStyles.objectField);
        //public static GUIStyle ObjectFieldThumb => new (EditorStyles.objectFieldThumb);
        //public static GUIStyle ObjectFieldMiniThumb => new (EditorStyles.objectFieldMiniThumb);
        //public static GUIStyle ColorField => new (EditorStyles.colorField);
        //public static GUIStyle LayerMaskField => new (EditorStyles.layerMaskField);

        //public static GUIStyle Toggle => new (EditorStyles.toggle);
        //public static GUIStyle Foldout => new (EditorStyles.foldout);
        //public static GUIStyle FoldoutPreDrop => new (EditorStyles.foldoutPreDrop);
        //public static GUIStyle FoldoutHeader => new (EditorStyles.foldoutHeader);
        //public static GUIStyle FoldoutHeaderIcon => new (EditorStyles.foldoutHeaderIcon);
        //public static GUIStyle ToggleGroup => new (EditorStyles.toggleGroup);

        //public static GUIStyle Toolbar => new (EditorStyles.toolbar);
        //public static GUIStyle ToolbarButton => new (EditorStyles.toolbarButton);
        //public static GUIStyle ToolbarPopup => new (EditorStyles.toolbarPopup);
        //public static GUIStyle ToolbarDropDown => new (EditorStyles.toolbarDropDown);
        //public static GUIStyle ToolbarTextField => new (EditorStyles.toolbarTextField);
        //public static GUIStyle ToolbarSearchField => new (EditorStyles.toolbarSearchField);

        //public static GUIStyle InspectorDefaultMargins => new (EditorStyles.inspectorDefaultMargins);
        //public static GUIStyle InspectorFullWidthMargins => new (EditorStyles.inspectorFullWidthMargins);

        //public static GUIStyle HelpBox => new (EditorStyles.helpBox);
        //public static GUIStyle IconButton => new (EditorStyles.iconButton);
        //public static GUIStyle SelectionRect => new (EditorStyles.selectionRect);
    }

    public static class Label
    {
        public static void Draw(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            Draw(new GUIContent(text), style, options);
        }

        public static void Draw(GUIContent content, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
                EditorGUILayout.LabelField(content, style, options);
            else
                EditorGUILayout.LabelField(content, options);
        }

        public static void DrawAutoSized(string text, GUIStyle style = null, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            style ??= LabelStyles.Label;
            style.alignment = anchor;
            var content = new GUIContent(text);
            var width   = style.CalcSize(content).x + 8f;
            Draw(content, style, GUILayout.Width(width));
        }

        /// <summary>
        /// Draws a truncated, tooltip‐powered, existence‐colored label that fits within the given width.
        /// </summary>
        /// <param name="fullText">The original, un‐truncated text.</param>
        /// <param name="tooltip">Hover tooltip showing the full text or missing path.</param>
        /// <param name="availableWidth">The pixel width available for this label.</param>
        /// <param name="averageCharWidth">Your average char‐width used for estimating truncation.</param>
        /// <param name="exists">If false, text is drawn red; otherwise white.</param>
        public static void DrawTruncatedLabel(string fullText, string tooltip, float availableWidth, float averageCharWidth, bool exists)
        {
            // 1) estimate how many chars will fit, then truncate
            int maxChars = Utils.TextUtils.EstimateMaxChars(availableWidth, averageCharWidth);
            string truncated = Utils.TextUtils.TruncateWithEllipsis(fullText, maxChars);

            // 2) prepare content & style
            var content = new GUIContent(truncated, tooltip);
            var style = LabelStyles.Label;
            style.fixedWidth = availableWidth;
            style.normal.textColor = exists ? Color.white : Color.red;

            // 3) render
            Draw(content, style);
        }

    }
}
#endif
