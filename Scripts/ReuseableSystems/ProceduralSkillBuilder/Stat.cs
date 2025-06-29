// --- Stat.cs ---
using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    [CreateAssetMenu(menuName = "RPG/Stat Data", fileName = "StatData")]
    public class Stat : ScriptableObject
    {
        public List<StatEntry> stats = new();
        public List<ResistanceEntry> resistances = new();

        public Dictionary<StatType, float> ToStatDictionary()
        {
            var dict = new Dictionary<StatType, float>();
            foreach (var s in stats)
                dict[s.Type] = s.Value;
            return dict;
        }

        public Dictionary<DamageType, float> ToResistanceDictionary()
        {
            var dict = new Dictionary<DamageType, float>();
            foreach (var r in resistances)
                dict[r.Type] = r.Multiplier;
            return dict;
        }
    }

    [System.Serializable]
    public struct StatEntry
    {
        public StatType Type;
        public float Value;
    }

    [System.Serializable]
    public struct ResistanceEntry
    {
        public DamageType Type;
        public float Multiplier;
    }
}