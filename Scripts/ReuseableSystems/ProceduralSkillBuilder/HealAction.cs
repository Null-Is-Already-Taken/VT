using UnityEngine;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    /// <summary>
    /// Action that heals targets based on configured parameters.
    /// </summary>
    public class HealAction : MultiTargetAction
    {
        private float baseHeal;
        private StatType? scalingStat;
        private float scalingMultiplier = 1f;
        private bool canOverheal = false;
        private float overhealLimit = 1.5f; // Can heal up to 150% of max HP by default

        public HealAction()
        {
        }

        public HealAction(float baseHeal, StatType? scalingStat = null, float scalingMultiplier = 1f, bool canOverheal = false)
        {
            this.baseHeal = baseHeal;
            this.scalingStat = scalingStat;
            this.scalingMultiplier = scalingMultiplier;
            this.canOverheal = canOverheal;
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            var targets = GetTargets(context);

            float finalHeal = CalculateFinalHeal(context, logger);

            foreach (var target in targets)
            {
                if (target == null) continue;

                var health = target.GetComponent<IHealth>();
                if (health == null) continue;

                float healAmount = finalHeal;
                
                // Apply overheal logic
                if (!canOverheal)
                {
                    float maxHealPossible = health.MaxHP - health.CurrentHP;
                    healAmount = Mathf.Min(healAmount, maxHealPossible);
                }
                else
                {
                    float maxHealPossible = health.MaxHP * overhealLimit - health.CurrentHP;
                    healAmount = Mathf.Min(healAmount, maxHealPossible);
                }

                if (healAmount > 0)
                {
                    health.Heal(healAmount);
                    logger?.LogTag("Heal", $"{target.name} healed for {healAmount}");
                }
                else
                {
                    logger?.LogTag("Heal", $"{target.name} cannot be healed (at max HP)");
                }
            }
        }

        private float CalculateFinalHeal(ExecutionContext context, ISkillLogger logger)
        {
            float finalHeal = baseHeal;

            if (scalingStat.HasValue)
            {
                var stats = context.Source?.GetComponent<IStatsProvider>();
                if (stats != null)
                {
                    float statValue = stats.GetStat(scalingStat.Value);
                    finalHeal += statValue * scalingMultiplier;
                    logger?.LogTag("Heal", $"{baseHeal} + {scalingStat.Value}({statValue}) x {scalingMultiplier} = {finalHeal}");
                }
            }

            return finalHeal;
        }

        public override string Description => $"Heal {baseHeal} HP" + 
            (scalingStat.HasValue ? $" + {scalingStat.Value} x {scalingMultiplier}" : "") +
            (canOverheal ? " (can overheal)" : "");

        // Fluent API methods for building the action
        public HealAction Base(float value)
        {
            baseHeal = value;
            return this;
        }

        public HealAction ScaleWith(StatType stat, float multiplier)
        {
            scalingStat = stat;
            scalingMultiplier = multiplier;
            return this;
        }

        public HealAction AllowOverheal(bool allow = true, float limit = 1.5f)
        {
            canOverheal = allow;
            overhealLimit = limit;
            return this;
        }

        public HealAction PreventOverheal()
        {
            canOverheal = false;
            return this;
        }
    }
} 