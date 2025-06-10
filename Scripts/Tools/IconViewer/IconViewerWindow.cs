#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VT.EditorUtils;

namespace VT.Tools.IconViewer
{
    public class IconViewerWindow : EditorWindow
    {
        private string byteInput = "";
        private Texture2D previewTexture;

        [MenuItem("Tools/Icon Viewer")]
        public static void ShowWindow()
        {
            GetWindow<IconViewerWindow>("Icon Viewer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Paste Value", EditorStyles.boldLabel);
            byteInput = EditorGUILayout.TextArea(byteInput, GUILayout.Height(100));

            if (GUILayout.Button("Preview Icon"))
            {
                previewTexture = ConvertToTexture(byteInput);
            }

            GUILayout.Space(10);
            if (previewTexture != null)
            {
                float size = Mathf.Min(position.width - 20, 128);
                GUILayout.Label(previewTexture, GUILayout.Width(size), GUILayout.Height(size));
            }
        }

        private Texture2D ConvertToTexture(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            try
            {
                var tokens = input
                    .Split(new[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToArray();

                // Heuristically detect if this looks like a byte array (hex or decimal)
                bool likelyByteArray = tokens.Length > 0 && tokens.All(t =>
                    (t.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && t.Length > 2 && IsHex(t.Substring(2))) ||
                    byte.TryParse(t, out _)
                );

                if (likelyByteArray)
                {
                    byte[] bytes = tokens.Select(s =>
                    {
                        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            return Convert.ToByte(s, 16);
                        else
                            return Convert.ToByte(s);
                    }).ToArray();

                    return IconLoader.Load1BitBitmap(bytes, 32, 32);
                }
                else
                {
                    return IconLoader.LoadTextureFromBase64(input);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to convert image input: " + e.Message);
                return null;
            }
        }

        private bool IsHex(string s)
        {
            return s.All(c => "0123456789abcdefABCDEF".Contains(c));
        }

    }
}

#endif
