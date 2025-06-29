using UnityEngine;
using System.Collections.Generic;

namespace VT.ReusableSystems.ProceduralSkillBuilder.Examples
{
    /// <summary>
    /// Example of a custom action that spawns projectiles.
    /// </summary>
    public class SpawnProjectileAction : MultiTargetAction
    {
        private GameObject projectilePrefab;
        private float speed = 10f;
        private float lifetime = 5f;
        private bool homing = false;

        public SpawnProjectileAction(GameObject prefab)
        {
            projectilePrefab = prefab;
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            var source = context.Source;
            var targets = GetTargets(context);

            if (source == null || projectilePrefab == null) return;

            foreach (var target in targets)
            {
                if (target == null) continue;

                // Spawn projectile
                GameObject projectile = Object.Instantiate(projectilePrefab, source.transform.position, Quaternion.identity);
                
                // Configure projectile behavior
                var projectileComponent = projectile.GetComponent<ProjectileBehavior>();
                if (projectileComponent != null)
                {
                    projectileComponent.Initialize(target, speed, lifetime, homing);
                }

                logger?.LogTag("Projectile", $"Spawned projectile at {source.name} targeting {target.name}");
            }
        }

        public override string Description => $"Spawn projectile(s) at {speed} speed" +
            (homing ? " (homing)" : "") + $" for {lifetime}s";

        // Fluent API
        public SpawnProjectileAction Speed(float speed)
        {
            this.speed = speed;
            return this;
        }

        public SpawnProjectileAction Lifetime(float seconds)
        {
            lifetime = seconds;
            return this;
        }

        public SpawnProjectileAction Homing(bool homing = true)
        {
            this.homing = homing;
            return this;
        }
    }

    /// <summary>
    /// Example of a custom action that creates a buff zone.
    /// </summary>
    public class CreateBuffZoneAction : SingleTargetAction
    {
        private GameObject zonePrefab;
        private float radius = 5f;
        private float duration = 10f;
        private string buffType = "Speed";
        private float buffValue = 1.5f;

        public CreateBuffZoneAction(GameObject prefab)
        {
            zonePrefab = prefab;
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            var source = context.Source;
            var target = GetTarget(context);

            if (source == null || zonePrefab == null) return;

            Vector3 position = target != null ? target.transform.position : source.transform.position;
            
            // Create buff zone
            GameObject zone = Object.Instantiate(zonePrefab, position, Quaternion.identity);
            
            // Configure zone behavior
            var zoneComponent = zone.GetComponent<BuffZoneBehavior>();
            if (zoneComponent != null)
            {
                zoneComponent.Initialize(radius, duration, buffType, buffValue);
            }

            logger?.LogTag("BuffZone", $"Created {buffType} buff zone at {position} for {duration}s");
        }

        public override string Description => $"Create {buffType} buff zone (radius: {radius}, duration: {duration}s)";

        // Fluent API
        public CreateBuffZoneAction Radius(float radius)
        {
            this.radius = radius;
            return this;
        }

        public CreateBuffZoneAction Duration(float seconds)
        {
            duration = seconds;
            return this;
        }

        public CreateBuffZoneAction BuffType(string type)
        {
            buffType = type;
            return this;
        }

        public CreateBuffZoneAction BuffValue(float value)
        {
            buffValue = value;
            return this;
        }
    }

    /// <summary>
    /// Example of a custom action that performs a combo attack.
    /// </summary>
    public class ComboAttackAction : MultiTargetAction
    {
        private List<IAction> comboActions = new List<IAction>();
        private float comboDelay = 0.2f;

        public ComboAttackAction()
        {
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            
            logger?.LogTag("Combo", $"Executing {comboActions.Count} combo attacks");

            // Execute each action in the combo
            foreach (var action in comboActions)
            {
                action.Execute(context);
                
                // In a real implementation, you might want to add delays between combo hits
                // This is a simplified version
            }
        }

        public override string Description => $"Combo attack with {comboActions.Count} hits";

        // Fluent API
        public ComboAttackAction AddAction(IAction action)
        {
            comboActions.Add(action);
            return this;
        }

        public ComboAttackAction Delay(float seconds)
        {
            comboDelay = seconds;
            return this;
        }

        // Convenience methods for common combo patterns
        public ComboAttackAction Slash(float damage, StatType stat = StatType.Strength)
        {
            return AddAction(new DamageAction(damage, stat, 1.0f, DamageType.Physical));
        }

        public ComboAttackAction Thrust(float damage, StatType stat = StatType.Dexterity)
        {
            return AddAction(new DamageAction(damage, stat, 1.2f, DamageType.Physical));
        }

        public ComboAttackAction Finisher(float damage, StatType stat = StatType.Strength)
        {
            return AddAction(new DamageAction(damage, stat, 2.0f, DamageType.Physical));
        }
    }

    // Example helper classes (these would be implemented elsewhere in your game)
    public class ProjectileBehavior : MonoBehaviour
    {
        public void Initialize(GameObject target, float speed, float lifetime, bool homing)
        {
            // Implementation would handle projectile movement, collision, etc.
        }
    }

    public class BuffZoneBehavior : MonoBehaviour
    {
        public void Initialize(float radius, float duration, string buffType, float buffValue)
        {
            // Implementation would handle buff zone effects, duration, etc.
        }
    }
} 