using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public interface IStatsProvider
    {
        /// <summary>
        /// Returns the value of the given stat (e.g., Strength, Intelligence).
        /// </summary>
        float GetStat(StatType stat);

        /// <summary>
        /// Returns a resistance multiplier for the given damage type.
        /// Example: 0.8 = 20% resistance, 1.2 = 20% weakness.
        /// </summary>
        float GetResistance(DamageType damageType);

        /// <summary>
        /// Optional: Override or extend resistances dynamically.
        /// </summary>
        void SetResistance(DamageType type, float multiplier);
    }

    // --- IStatusReceiver.cs ---
    public interface IStatusReceiver
    {
        void ApplyStatus(string status, float duration);
    }

    // --- ISkillLogger.cs ---
    public interface ISkillLogger
    {
        void Log(string message);
        void LogTag(string tag, string message);
    }

    // --- ISkillStep.cs ---
    public interface ISkillStep
    {
        void Execute(ExecutionContext context);
        string Description { get; }
    }

    public interface ITargetFinder
    {
        List<GameObject> ResolveTargets(GameObject source);

        /// <summary>
        /// A short description of how this finder works (for debugging/logging).
        /// </summary>
        string Description { get; }
    }
}