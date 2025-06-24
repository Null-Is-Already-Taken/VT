#if DOTWEEN

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using VT.Extensions;

namespace VT.ReusableSystems.UI.Legacy
{
    public interface IUITransitionEffect
    {
        Tween Play(Transform target);
        Tween Rewind(Transform target);
    }

    public enum UITransitionType
    {
        None,
        ScaleBounce,
        FadeIn,
        SlideFromRight
    }

    public abstract class BaseEffect : IUITransitionEffect
    {
        public BaseEffect(float duration)
        {
            this.duration = duration;
        }

        protected float duration;

        public abstract Tween Play(Transform target);

        public abstract Tween Rewind(Transform target);
    }

    public class ScaleBounceEffect : BaseEffect
    {
        public ScaleBounceEffect(float duration) : base(duration)
        {
        }

        public override Tween Play(Transform target)
        {
            if (target == null) return null;

            target.localScale = Vector3.zero;
            return target.DOScale(1f, duration).SetEase(Ease.OutBack);
        }

        public override Tween Rewind(Transform target)
        {
            if (target == null) return null;

            return target.DOScale(0f, duration).SetEase(Ease.InBack);
        }
    }

    public class FadeInEffect : BaseEffect
    {
        public FadeInEffect(float duration) : base(duration)
        {
        }

        public override Tween Play(Transform target)
        {
            if (target.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = 0f;
                return canvasGroup.DOFade(1f, duration);
            }
            return null;
        }

        public override Tween Rewind(Transform target)
        {
            if (target.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                return canvasGroup.DOFade(0f, duration);
            }
            return null;
        }
    }


    public class SlideInFromRightEffect : BaseEffect
    {
        public SlideInFromRightEffect(float duration) : base(duration)
        {
        }

        public override Tween Play(Transform target)
        {
            if (target == null) return null;

            var startPos = target.localPosition + new Vector3(800, 0, 0);
            target.localPosition = startPos;
            return target.DOLocalMoveX(startPos.x - 800, duration).SetEase(Ease.OutExpo);
        }

        public override Tween Rewind(Transform target)
        {
            if (target == null) return null;

            return target.DOLocalMoveX(target.localPosition.x + 800, duration).SetEase(Ease.InExpo);
        }
    }

    // ───── Effect Registry ─────

    public static class TransitionEffects
    {
        public static readonly Dictionary<UITransitionType, IUITransitionEffect> Effects = new()
        {
            { UITransitionType.ScaleBounce, new ScaleBounceEffect(1f) },
            { UITransitionType.FadeIn, new FadeInEffect(1f) },
            { UITransitionType.SlideFromRight, new SlideInFromRightEffect(1f) }
        };
    }
}

#endif // DOTWEEN
