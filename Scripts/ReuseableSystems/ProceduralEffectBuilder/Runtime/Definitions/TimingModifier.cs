using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Modifies when or how the effect is executed over time.
    /// Wraps a base IEffect with additional timing behavior.
    /// </summary>
    public abstract class TimingModifier : ScriptableObject
    {
        /// <summary>
        /// Wraps the given effect with time-based behavior (e.g. delay, DoT).
        /// </summary>
        /// <param name="effect">The compiled effect to modify.</param>
        /// <returns>A new IEffect that executes with timing logic.</returns>
        public abstract IEffect ApplyTiming(IEffect effect);
    }
}
