using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// A conditional effect composition that executes subEffects if a condition is true,
    /// otherwise executes a fallback effect.
    /// </summary>
    [CreateAssetMenu(menuName = "Procedural Effect Builder/Composition/Conditional")]
    public class ConditionalComposition : EffectComposition
    {
        [Header("Condition")]
        public ConditionDefinition condition;

        [Header("Fallback (if condition is false)")]
        public EffectDefinition fallbackEffect;

        public override IEffect Compile(GameObject source, GameObject target)
        {
            bool result = condition != null && condition.Evaluate(source, target);

            if (result)
            {
                return CompileSubChain(source, target);
            }
            else if (fallbackEffect != null)
            {
                return fallbackEffect.Compile(source, target);
            }

            return new NoOpEffect();
        }

        private IEffect CompileSubChain(GameObject source, GameObject target)
        {
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
