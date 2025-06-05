using UnityEngine;
using UnityEngine.Video;

namespace VT.UI
{
    public interface ITextHook
    {
        void UpdateText(string text);
    }

    public interface IBoolHook
    {
        void UpdateBool(bool value);
    }

    public interface IImageHook
    {
        void UpdateImage(Sprite sprite);
    }

    public interface IVideoHook
    {
        void PlayVideo(VideoClip clip);
        void StopVideo();
    }
}