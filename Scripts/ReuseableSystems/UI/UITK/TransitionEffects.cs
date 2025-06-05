using UnityEngine;
using UnityEngine.UIElements;

namespace VT.ReuseableSystems.UI.UITK
{
    public interface IUITransitionEffect
    {
        void Play(VisualElement element);
        void Rewind(VisualElement element);
    }

    public enum UITransitionType
    {
        None,
        FadeInOut,
        SlideLeftRight,
        SlideRightLeft,
        SlideUpDown,
        SlideDownUp,
        Custom
    }

    public class FadeTransitionEffect : IUITransitionEffect
    {
        private readonly int durationInMS;

        public FadeTransitionEffect(float duration)
        {
            durationInMS = (int)(duration * 100);
        }

        public void Play(VisualElement element)
        {
            element.style.opacity = 0;
            element.style.display = DisplayStyle.Flex;
            element.experimental.animation.Start(0f, 1f, durationInMS, (e, v) =>
            {
                e.style.opacity = v;
            });
        }

        public void Rewind(VisualElement element)
        {
            element.experimental.animation.Start(1f, 0f, durationInMS, (e, v) =>
            {
                e.style.opacity = v;
            }).OnCompleted(() =>
            {
                element.style.display = DisplayStyle.None;
            });
        }
    }

    public class SlideTransitionEffect : IUITransitionEffect
    {
        private readonly Vector2 from;
        private readonly Vector2 to;
        private readonly int durationInMS;

        public SlideTransitionEffect(Vector2 from, Vector2 to , float duration)
        {
            this.from = from;
            this.to = to;
            durationInMS = (int)(duration * 100);
        }

        public void Play(VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
            element.experimental.animation.Start(
                from: from,
                to: to,
                durationMs: durationInMS,
                onValueChanged: (e, v) => { }
            );
        }

        public void Rewind(VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
            element.experimental.animation.Start(
                from: from,
                to: to,
                durationMs: durationInMS,
                onValueChanged: (e, v) => { }
            ).OnCompleted(() =>
            {
                element.style.display = DisplayStyle.None;
            });
        }
    }
}
