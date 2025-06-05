using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace VT.UI
{
    public class VideoHook : IVideoHook
    {
        private readonly VideoPlayer player;
        private readonly RawImage output;
        private readonly Vector2Int texSize;

        public VideoHook(VideoPlayer player, RawImage output, Vector2Int texSize)
        {
            this.player = player;
            this.output = output;
            this.texSize = texSize;
        }

        public void PlayVideo(VideoClip clip)
        {
            if (player == null || output == null || clip == null)
            {
                player.gameObject.SetActive(false);
                output.gameObject.SetActive(false);
                return;
            }

            player.clip = clip;
            player.renderMode = VideoRenderMode.RenderTexture;

            RenderTexture rt = new(texSize.x, texSize.y, 0);
            player.targetTexture = rt;
            output.texture = rt;

            player.gameObject.SetActive(true);
            output.gameObject.SetActive(true);

            player.Play();
        }

        public void StopVideo()
        {
            if (player != null)
            {
                player.Stop();
                player.targetTexture = null;
                player.gameObject.SetActive(false);
            }

            if (output != null)
            {
                output.texture = null;
                output.gameObject.SetActive(false);
            }
        }
    }
}
