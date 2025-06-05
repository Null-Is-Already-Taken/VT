using UnityEngine;
using UnityEngine.UI;

namespace VT.UI
{
    public class ImageHook : IImageHook
    {
        private readonly Image image;

        public ImageHook(Image image)
        {
            this.image = image;
        }

        public void UpdateImage(Sprite sprite)
        {
            if (image != null)
            {
                image.sprite = sprite;
                image.gameObject.SetActive(sprite != null);
            }
        }
    }
}
