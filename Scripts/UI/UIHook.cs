using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VT.Utils;

namespace VT.UI
{
    public class UIHook : MonoBehaviour
    {
        // ---------- TEXT ----------
        [SerializeField] protected TMP_Text textField;

        protected ITextHook textHook;
        protected ITextHook TextHook => LazyGetter.InitHook(ref textHook, () => new TMPTextHook(textField));

        public void UpdateText(string text)
        {
            TextHook?.UpdateText($"{text}");
        }

        // ---------- TOGGLE ----------
        [SerializeField] protected Toggle boolField;

        protected IBoolHook toggleHook;
        protected IBoolHook ToggleHook => LazyGetter.InitHook(ref toggleHook, () => new ToggleHook(boolField));

        public void UpdateBool(bool value)
        {
            ToggleHook?.UpdateBool(value);
        }

        // ---------- IMAGE ----------
        [SerializeField] protected Image imageField;

        protected IImageHook imageHook;
        protected IImageHook ImageHook => LazyGetter.InitHook(ref imageHook, () => new ImageHook(imageField));

        public void UpdateImage(Sprite sprite)
        {
            ImageHook?.UpdateImage(sprite);
        }

        // ---------- VIDEO ----------
        [SerializeField] protected VideoPlayer videoField;
        [SerializeField] protected RawImage videoOutputField;
        [SerializeField] protected Vector2Int textureSize;

        protected IVideoHook videoHook;
        protected IVideoHook VideoHook => LazyGetter.InitHook(ref videoHook, () => new VideoHook(videoField, videoOutputField, textureSize));

        public void UpdateVideo(VideoClip clip)
        {
            VideoHook?.PlayVideo(clip);
        }

        public void StopVideo()
        {
            videoHook?.StopVideo();
        }
    }
}
