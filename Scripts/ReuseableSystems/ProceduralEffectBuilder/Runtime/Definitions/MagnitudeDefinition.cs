using UnityEngine;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Base class for defining how the magnitude (value) of an effect is calculated.
    /// </summary>
    public abstract class MagnitudeDefinition : ScriptableObject
    {
        /// <summary>
        /// Computes the magnitude of the effect based on source and target.
        /// </summary>
        /// <param name="source">The GameObject applying the effect.</param>
        /// <param name="target">The GameObject receiving the effect.</param>
        /// <returns>The computed value (e.g. damage, heal amount, duration).</returns>
        public abstract float Evaluate(GameObject source, GameObject target);
    }
}
