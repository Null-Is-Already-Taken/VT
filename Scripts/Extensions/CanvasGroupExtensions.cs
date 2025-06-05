using UnityEngine;
using DG.Tweening;

namespace VT.Extensions
{
    public static class CanvasGroupExtensions
    {
        /// <summary>
        /// Manually fades the CanvasGroup's alpha using DOTween.To.
        /// </summary>
        /// <param name="canvasGroup">The CanvasGroup to fade.</param>
        /// <param name="endValue">The target alpha value.</param>
        /// <param name="duration">The fade duration.</param>
        /// <returns>The Tween controlling the animation.</returns>
        public static Tween DOFade(this CanvasGroup canvasGroup, float endValue, float duration)
        {
            return DOTween.To(
                getter: () => canvasGroup.alpha,
                setter: x => canvasGroup.alpha = x,
                endValue: endValue,
                duration: duration
            );
        }
    }
}
