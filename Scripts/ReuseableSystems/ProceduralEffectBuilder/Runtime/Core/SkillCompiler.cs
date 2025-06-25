using System.Collections.Generic;
using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core
{
    public static class SkillCompiler
    {
        /// <summary>
        /// Compiles a SkillTemplate into a runtime IEffect chain.
        /// </summary>
        /// <param name="template">The skill template.</param>
        /// <param name="context">The object triggering the skill (e.g., caster, trap).</param>
        /// <returns>A compiled IEffect chain.</returns>
        public static IEffect Compile(SkillTemplate template, GameObject context)
        {
            // Resolve source and targets
            GameObject source = template.source.ResolveSource(context);
            List<GameObject> targets = template.target.ResolveTargets(source);

            IEffect finalChain = null;

            // Use composition override (e.g. sequence, conditional tree)
            if (template.compositionOverride != null)
            {
                return template.compositionOverride.Compile(source, null);
            }

            foreach (var target in targets)
            {
                // Step 1: Compile base effect
                IEffect effect = template.effect.Compile(source, target);

                // Step 2: Inject magnitude if needed
                if (template.magnitude != null && effect is IMagnitudeAware magnitudeAware)
                {
                    float value = template.magnitude.Evaluate(source, target);
                    magnitudeAware.SetMagnitude(value);
                }

                // Step 3: Wrap with feedback (VFX, SFX, etc.)
                if (template.feedbackDecorators != null && template.feedbackDecorators.Count > 0)
                {
                    effect = new FeedbackEffectWrapper(effect, template.feedbackDecorators, source, target);
                }

                // Step 4: Wrap with condition
                if (template.condition != null)
                {
                    bool result = template.condition.Evaluate(source, target);
                    effect = new ConditionalEffect(effect, () => result);
                }

                // Step 5: Apply timing (e.g., delay)
                if (template.timing != null)
                {
                    effect = template.timing.ApplyTiming(effect);
                }

                // Step 6: Add to chain
                finalChain = finalChain == null ? effect : finalChain.And(effect);
            }

            return finalChain ?? new NoOpEffect();
        }
    }
}
