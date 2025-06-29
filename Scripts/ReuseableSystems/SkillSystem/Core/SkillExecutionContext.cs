using System;
using System.Collections.Generic;
using UnityEngine;
using VT.ReusableSystems.Events;

namespace VT.ReusableSystems.SkillSystem.Core
{
    /// <summary>
    /// Context object that provides all necessary data and services to skill blocks during execution.
    /// This ensures skill blocks have access to everything they need without tight coupling.
    /// </summary>
    public class SkillExecutionContext
    {
        /// <summary>
        /// The entity that is executing the skill.
        /// </summary>
        public GameObject Caster { get; set; }
        
        /// <summary>
        /// The target of the skill (can be null for self-targeted skills).
        /// </summary>
        public GameObject Target { get; set; }
        
        /// <summary>
        /// The skill being executed.
        /// </summary>
        public SkillDefinition Skill { get; set; }
        
        /// <summary>
        /// The skill instance being executed.
        /// </summary>
        public SkillInstance SkillInstance { get; set; }
        
        /// <summary>
        /// Current time when the skill is being executed.
        /// </summary>
        public float ExecutionTime { get; set; }
        
        /// <summary>
        /// Delta time since last frame.
        /// </summary>
        public float DeltaTime { get; set; }
        
        /// <summary>
        /// Input data from the player or AI.
        /// </summary>
        public SkillInputData InputData { get; set; }
        
        /// <summary>
        /// Shared data that persists across all blocks in a skill execution.
        /// </summary>
        public Dictionary<string, object> SharedData { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Event bus for raising skill-related events.
        /// </summary>
        public IEventBus EventBus { get; set; }
        
        /// <summary>
        /// Camera reference for targeting and positioning.
        /// </summary>
        public Camera Camera { get; set; }
        
        /// <summary>
        /// Physics layer mask for raycasting and targeting.
        /// </summary>
        public LayerMask PhysicsLayerMask { get; set; }
        
        /// <summary>
        /// Random number generator for skill effects.
        /// </summary>
        public System.Random Random { get; set; }

        /// <summary>
        /// Get shared data of a specific type.
        /// </summary>
        public T GetSharedData<T>(string key, T defaultValue = default(T))
        {
            if (SharedData.TryGetValue(key, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Set shared data that can be accessed by other blocks.
        /// </summary>
        public void SetSharedData<T>(string key, T value)
        {
            SharedData[key] = value;
        }

        /// <summary>
        /// Check if shared data exists.
        /// </summary>
        public bool HasSharedData(string key)
        {
            return SharedData.ContainsKey(key);
        }

        /// <summary>
        /// Remove shared data.
        /// </summary>
        public void RemoveSharedData(string key)
        {
            SharedData.Remove(key);
        }

        /// <summary>
        /// Clear all shared data.
        /// </summary>
        public void ClearSharedData()
        {
            SharedData.Clear();
        }

        /// <summary>
        /// Get a random float between min and max.
        /// </summary>
        public float RandomFloat(float min, float max)
        {
            return min + (float)Random.NextDouble() * (max - min);
        }

        /// <summary>
        /// Get a random integer between min and max (inclusive).
        /// </summary>
        public int RandomInt(int min, int max)
        {
            return Random.Next(min, max + 1);
        }

        /// <summary>
        /// Get a random boolean with the given probability.
        /// </summary>
        public bool RandomBool(float probability = 0.5f)
        {
            return Random.NextDouble() < probability;
        }

        /// <summary>
        /// Get a random element from a list.
        /// </summary>
        public T RandomElement<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);
            
            return list[Random.Next(list.Count)];
        }
    }

    /// <summary>
    /// Input data for skill execution.
    /// </summary>
    public class SkillInputData
    {
        /// <summary>
        /// Mouse position in screen coordinates.
        /// </summary>
        public Vector2 MousePosition { get; set; }
        
        /// <summary>
        /// Mouse position in world coordinates.
        /// </summary>
        public Vector3 MouseWorldPosition { get; set; }
        
        /// <summary>
        /// Direction vector for directional skills.
        /// </summary>
        public Vector3 Direction { get; set; }
        
        /// <summary>
        /// Whether the skill should be charged (held down).
        /// </summary>
        public bool IsCharged { get; set; }
        
        /// <summary>
        /// Charge level (0-1) for charged skills.
        /// </summary>
        public float ChargeLevel { get; set; }
        
        /// <summary>
        /// Additional input parameters.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Get a parameter of a specific type.
        /// </summary>
        public T GetParameter<T>(string key, T defaultValue = default(T))
        {
            if (Parameters.TryGetValue(key, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Set a parameter.
        /// </summary>
        public void SetParameter<T>(string key, T value)
        {
            Parameters[key] = value;
        }
    }

    /// <summary>
    /// Simple event bus interface for skill events.
    /// </summary>
    public interface IEventBus
    {
        void Raise<T>(T eventData) where T : IEvent;
    }
} 