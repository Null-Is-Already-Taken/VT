using UnityEngine;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    /// <summary>
    /// Action that deals damage to targets based on configured parameters.
    /// </summary>
    public class DamageAction : MultiTargetAction
    {
        private float baseDamage;
        private StatType? scalingStat;
        private float scalingMultiplier = 1f;
        private DamageType damageType = DamageType.Physical;
        private SkillCategory? skillCategory;

        public DamageAction()
        {
        }

        public DamageAction(float baseDamage, StatType? scalingStat = null, float scalingMultiplier = 1f, DamageType damageType = DamageType.Physical)
        {
            this.baseDamage = baseDamage;
            this.scalingStat = scalingStat;
            this.scalingMultiplier = scalingMultiplier;
            this.damageType = damageType;
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            var targets = GetTargets(context);

            Debug.Log($"[DamageAction] Executing damage action. Base damage: {baseDamage}, Targets count: {targets?.Count ?? 0}");
            Debug.Log($"[DamageAction] Source: {context.Source?.name ?? "null"}");

            float finalDamage = CalculateFinalDamage(context, logger);
            Debug.Log($"[DamageAction] Final calculated damage: {finalDamage}");

            if (targets == null || targets.Count == 0)
            {
                Debug.LogWarning("[DamageAction] No targets found!");
                return;
            }

            foreach (var target in targets)
            {
                if (target == null) 
                {
                    Debug.LogWarning("[DamageAction] Target is null, skipping...");
                    continue;
                }

                Debug.Log($"[DamageAction] Processing target: {target.name}");

                float resist = target.GetComponent<IStatsProvider>()?.GetResistance(damageType) ?? 1f;
                float result = finalDamage * (1 - resist);
                
                Debug.Log($"[DamageAction] Target {target.name} - Resistance: {resist}, Final damage: {result}");
                
                logger?.LogTag("Damage", $"{target.name} takes {result} damage (resist: {resist})");
                
                var health = target.GetComponent<IHealth>();
                if (health != null)
                {
                    health.TakeDamage(result);
                    Debug.Log($"[DamageAction] Applied {result} damage to {target.name}. Current HP: {health.CurrentHP}");
                }
                else
                {
                    Debug.LogWarning($"[DamageAction] Target {target.name} has no IHealth component!");
                }
            }
        }

        private float CalculateFinalDamage(ExecutionContext context, ISkillLogger logger)
        {
            float finalDamage = baseDamage;

            if (scalingStat.HasValue)
            {
                var stats = context.Source?.GetComponent<IStatsProvider>();
                if (stats != null)
                {
                    float statValue = stats.GetStat(scalingStat.Value);
                    finalDamage += statValue * scalingMultiplier;
                    logger?.LogTag("Damage", $"{baseDamage} + {scalingStat.Value}({statValue}) x {scalingMultiplier} = {finalDamage}");
                    Debug.Log($"[DamageAction] Scaling: {baseDamage} + {scalingStat.Value}({statValue}) x {scalingMultiplier} = {finalDamage}");
                }
                else
                {
                    Debug.LogWarning($"[DamageAction] Source {context.Source?.name ?? "null"} has no IStatsProvider component!");
                }
            }

            return finalDamage;
        }

        public override string Description => $"Deal {baseDamage} {damageType} damage" + 
            (scalingStat.HasValue ? $" + {scalingStat.Value} x {scalingMultiplier}" : "");

        // Fluent API methods for building the action
        public DamageAction Base(float value)
        {
            baseDamage = value;
            return this;
        }

        public DamageAction ScaleWith(StatType stat, float multiplier)
        {
            scalingStat = stat;
            scalingMultiplier = multiplier;
            return this;
        }

        public DamageAction AsType(DamageType type)
        {
            damageType = type;
            return this;
        }

        public DamageAction InCategory(SkillCategory category)
        {
            skillCategory = category;
            return this;
        }
    }

    /// <summary>
    /// Legacy Damage class for backward compatibility - now wraps DamageAction.
    /// </summary>
    [System.Obsolete("Use DamageAction instead for better integration with the action system.")]
    public class Damage
    {
        private readonly ExecutionContext context;
        private readonly DamageAction action;

        public Damage(ExecutionContext ctx)
        {
            context = ctx;
            action = new DamageAction();
        }

        public Damage Base(float value)
        {
            action.Base(value);
            return this;
        }

        public Damage ScaleWith(StatType stat, float multiplier)
        {
            action.ScaleWith(stat, multiplier);
            return this;
        }

        public Damage AsType(DamageType type)
        {
            action.AsType(type);
            return this;
        }

        public Damage InCategory(SkillCategory category)
        {
            action.InCategory(category);
            return this;
        }

        public void Execute()
        {
            action.Execute(context);
        }
    }
}