// --- StatProvider.cs ---
using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class StatProvider : MonoBehaviour, IStatsProvider
    {
        [SerializeField] private Stat stat;

        private Dictionary<StatType, float> stats;
        private Dictionary<DamageType, float> resistances;

        private void Awake()
        {
            stats = stat.ToStatDictionary();
            resistances = stat.ToResistanceDictionary();
        }

        public float GetStat(StatType stat)
        {
            return stats.TryGetValue(stat, out var value) ? value : 0f;
        }

        public float GetResistance(DamageType damageType)
        {
            return Mathf.Clamp01(resistances.TryGetValue(damageType, out var value) ? value : 0f);
        }

        public void SetResistance(DamageType type, float multiplier)
        {
            resistances[type] = multiplier;
        }
    }
}
