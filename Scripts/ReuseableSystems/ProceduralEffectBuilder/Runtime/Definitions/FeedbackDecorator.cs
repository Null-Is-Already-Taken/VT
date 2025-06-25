using UnityEngine;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Adds cosmetic-only behavior to an effect (e.g., play VFX, sound, animation).
    /// Executed alongside the actual gameplay effect.
    /// </summary>
    public abstract class FeedbackDecorator : ScriptableObject
    {
        /// <summary>
        /// Called when the effect is triggered.
        /// This should never affect game logic (cosmetic only).
        /// </summary>
        /// <param name="source">The GameObject applying the effect.</param>
        /// <param name="target">The GameObject receiving the effect.</param>
        public abstract void Play(GameObject source, GameObject target);
    }
}
