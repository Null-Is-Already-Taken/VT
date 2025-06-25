using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Compositions
{
    /// <summary>
    /// Compiles multiple EffectDefinitions into a single chained IEffect.
    /// </summary>
    [CreateAssetMenu(menuName = "Procedural Effect Builder/Composition/Effect Sequence")]
    public class EffectSequence : EffectComposition
    {
        public override IEffect Compile(GameObject source, GameObject target)
        {
            if (subEffects == null || subEffects.Count == 0)
                return new NoOpEffect();

            IEffect chain = null;

            foreach (var def in subEffects)
            {
                if (def == null) continue;

                IEffect next = def.Compile(source, target);
                chain = chain == null ? next : chain.And(next);
            }

            return chain ?? new NoOpEffect();
        }
    }
}
