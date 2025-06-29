using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class ExecutionContext
    {
        public GameObject Source;
        public List<GameObject> Targets = new();
        public Dictionary<string, object> Variables { get; } = new();

        public T Get<T>(string key, T fallback = default)
        {
            if (Variables.TryGetValue(key, out var value) && value is T casted)
                return casted;
            return fallback;
        }

        public void Set<T>(string key, T value)
        {
            Variables[key] = value;
        }

        public bool Has<T>(string key)
        {
            return Variables.TryGetValue(key, out var value) && value is T;
        }
    }
}
