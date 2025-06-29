using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    /// <summary>
    /// Action that teleports the source to a target location or target.
    /// </summary>
    public class TeleportAction : SingleTargetAction
    {
        private TeleportType teleportType = TeleportType.ToTarget;
        private Vector3? customPosition;
        private float maxDistance = float.MaxValue;
        private bool requireLineOfSight = false;

        public enum TeleportType
        {
            ToTarget,        // Teleport to the target's position
            ToPosition,      // Teleport to a custom position
            BehindTarget,    // Teleport behind the target
            AwayFromTarget   // Teleport away from the target
        }

        public TeleportAction()
        {
        }

        public TeleportAction(TeleportType type, Vector3? position = null)
        {
            teleportType = type;
            customPosition = position;
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            var source = context.Source;
            var target = GetTarget(context);

            if (source == null)
            {
                logger?.LogTag("Teleport", "No source available for teleport");
                return;
            }

            Vector3 teleportPosition = CalculateTeleportPosition(source, target, logger);

            // Check distance limit
            float distance = Vector3.Distance(source.transform.position, teleportPosition);
            if (distance > maxDistance)
            {
                logger?.LogTag("Teleport", $"Teleport distance {distance} exceeds limit {maxDistance}");
                return;
            }

            // Check line of sight if required
            if (requireLineOfSight && target != null)
            {
                if (!HasLineOfSight(source.transform.position, teleportPosition))
                {
                    logger?.LogTag("Teleport", "No line of sight to teleport destination");
                    return;
                }
            }

            // Perform teleport
            source.transform.position = teleportPosition;
            logger?.LogTag("Teleport", $"{source.name} teleported to {teleportPosition}");
        }

        private Vector3 CalculateTeleportPosition(GameObject source, GameObject target, ISkillLogger logger)
        {
            switch (teleportType)
            {
                case TeleportType.ToTarget:
                    return target != null ? target.transform.position : source.transform.position;

                case TeleportType.ToPosition:
                    return customPosition ?? source.transform.position;

                case TeleportType.BehindTarget:
                    if (target == null) return source.transform.position;
                    Vector3 direction = (source.transform.position - target.transform.position).normalized;
                    return target.transform.position + direction * 2f; // 2 units behind target

                case TeleportType.AwayFromTarget:
                    if (target == null) return source.transform.position;
                    Vector3 awayDirection = (source.transform.position - target.transform.position).normalized;
                    return source.transform.position + awayDirection * 5f; // 5 units away

                default:
                    return source.transform.position;
            }
        }

        private bool HasLineOfSight(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);
            
            // Simple raycast check - you might want to use a more sophisticated method
            return !Physics.Raycast(from, direction, distance, LayerMask.GetMask("Obstacles"));
        }

        public override string Description => $"Teleport {teleportType}" +
            (customPosition.HasValue ? $" to {customPosition.Value}" : "") +
            (maxDistance < float.MaxValue ? $" (max {maxDistance}m)" : "") +
            (requireLineOfSight ? " (requires LOS)" : "");

        // Fluent API methods for building the action
        public TeleportAction Type(TeleportType type)
        {
            teleportType = type;
            return this;
        }

        public TeleportAction Position(Vector3 position)
        {
            customPosition = position;
            return this;
        }

        public TeleportAction MaxDistance(float distance)
        {
            maxDistance = distance;
            return this;
        }

        public TeleportAction RequireLineOfSight(bool require = true)
        {
            requireLineOfSight = require;
            return this;
        }
    }
} 