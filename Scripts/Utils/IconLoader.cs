using System;
using UnityEngine;

namespace VT.Utils
{
    public static class IconLoader
    {
        public static Texture2D LoadTextureFromBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            Texture2D tex = new(32, 32, TextureFormat.RGBA32, false);
            tex.LoadImage(data);
            tex.Apply();
            return tex;
        }

        public static Texture2D Load1BitBitmap(byte[] data, int width, int height)
        {
            Texture2D tex = new(width, height, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[width * height];

            for (int y = 0; y < height; y++) // Bottom-up
            {
                for (int x = 0; x < width; x++)
                {
                    int byteIndex = y * ((width + 7) / 8) + (x / 8);
                    if (byteIndex >= data.Length) continue;

                    int bitIndex = 7 - (x % 8);
                    bool isSet = (data[byteIndex] & (1 << bitIndex)) != 0;

                    pixels[(height - 1 - y) * width + x] = isSet ? Color.white : Color.clear;
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Loads a Texture2D from a Base64-encoded image string.
        /// </summary>
        public static Texture2D LoadTextureFromBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;

            try
            {
                byte[] data = Convert.FromBase64String(base64);
                return LoadTextureFromBytes(data);
            }
            catch (FormatException e)
            {
                Debug.LogError($"Base64 decode failed: {e.Message}");
                return null;
            }
        }
    }
}