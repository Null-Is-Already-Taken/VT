// --- SkillInterpreter.cs ---
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public static class SkillInterpreter
    {
        public static void Execute(ExecutionContext ctx)
        {
            var logger = ctx.Get<ISkillLogger>(ContextKeys.Logger, null);

            float baseDamage = ctx.Get<float>(ContextKeys.BaseDamage, 0f);
            float finalDamage = baseDamage;

            if (ctx.Get<StatType?>(ContextKeys.ScalingStat) is StatType scaleStat)
            {
                float multiplier = ctx.Get<float>(ContextKeys.ScalingMultiplier, 1f);
                var stats = ctx.Source.GetComponent<IStatsProvider>();
                float statValue = stats.GetStat(scaleStat);
                finalDamage += statValue * multiplier;
                logger?.LogTag("Damage", $"{baseDamage} + {scaleStat}({statValue}) x {multiplier} = {finalDamage}");
            }

            ctx.Set(ContextKeys.Amount, finalDamage);

            foreach (var target in ctx.Targets)
            {
                float resist = target.GetComponent<IStatsProvider>()?.GetResistance(ctx.Get<DamageType>(ContextKeys.DamageType)) ?? 1f;
                float result = finalDamage * (1 - resist);
                logger?.LogTag("Target", $"{target.name} resist {resist} → {result}");
                target.GetComponent<IHealth>()?.TakeDamage(result);
            }
        }
    }
}