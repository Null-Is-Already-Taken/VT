using UnityEngine;

namespace VT.SkillSystem
{
    /// <summary>
    /// Context data passed between skill blocks during execution
    /// </summary>
    public class SkillContext
    {
        public GameObject Caster { get; set; }
        public GameObject Target { get; set; }
        public Vector3 Position { get; set; }
        public float Magnitude { get; set; }
        public float Duration { get; set; }
        public float Range { get; set; }
        public LayerMask TargetLayer { get; set; }
        
        // Custom data storage for blocks to communicate
        private System.Collections.Generic.Dictionary<string, object> customData = new System.Collections.Generic.Dictionary<string, object>();
        
        public void SetData(string key, object value)
        {
            customData[key] = value;
        }
        
        public T GetData<T>(string key, T defaultValue = default(T))
        {
            if (customData.TryGetValue(key, out object value) && value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }
        
        public SkillContext Clone()
        {
            var clone = new SkillContext
            {
                Caster = this.Caster,
                Target = this.Target,
                Position = this.Position,
                Magnitude = this.Magnitude,
                Duration = this.Duration,
                Range = this.Range,
                TargetLayer = this.TargetLayer
            };
            
            foreach (var kvp in customData)
            {
                clone.customData[kvp.Key] = kvp.Value;
            }
            
            return clone;
        }
    }
} 