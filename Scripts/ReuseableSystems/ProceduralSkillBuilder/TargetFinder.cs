using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    [Flags]
    public enum TargetingMode
    {
        None = 0,
        Self = 1 << 0,
        Allies = 1 << 1,
        Enemies = 1 << 2,
        All = Self | Allies | Enemies
    }

    public class TargetFinder
    {
        private ITargetFinder strategy;

        /// <summary>Pick exactly this one target (if non‐null).</summary>
        public TargetFinder Single(GameObject target)
        {
            strategy = new SingleTargetFinder(target);
            return this;
        }

        /// <summary>Use a circular (radius) AoE.</summary>
        public TargetFinder Radius(float radius, TargetingMode mode = TargetingMode.Enemies)
        {
            strategy = new RadiusTargetFinder(radius, mode);
            return this;
        }

        /// <summary>Supply any custom finder.</summary>
        public TargetFinder Custom(ITargetFinder customFinder)
        {
            strategy = customFinder;
            return this;
        }

        /// <summary>Fallback to any other strategy you've already set.</summary>
        public ITargetFinder Build()
        {
            if (strategy == null)
                throw new InvalidOperationException("No TargetFinder strategy was specified.");
            return strategy;
        }
    }

    public class RadiusTargetFinder : ITargetFinder
    {
        private readonly float radius;
        public float Radius => radius;
        private readonly TargetingMode mode;

        public RadiusTargetFinder(float radius, TargetingMode mode)
        {
            this.radius = radius;
            this.mode = mode;
        }

        public string Description => $"Radius(r={radius}, mode={mode})";

        public List<GameObject> ResolveTargets(GameObject source)
        {
            Debug.Log($"[RadiusTargetFinder] Resolving targets for source: {source?.name ?? "null"}");
            Debug.Log($"[RadiusTargetFinder] Radius: {radius}, Mode: {mode}");
            
            var allTargets = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(target => TargetFilters.ComponentFilter<IHealth>(target));

            Debug.Log($"[RadiusTargetFinder] Found {allTargets.Count()} objects with IHealth component");

            var withinRadiusTargets = allTargets
                .Where(target => TargetFilters.WithinRadiusFilter(source, target, radius));

            Debug.Log($"[RadiusTargetFinder] Found {withinRadiusTargets.Count()} targets within radius");

            var validTargets = withinRadiusTargets
                .Where(target => TargetFilters.TargetingModeFilter(source, target, mode));

            Debug.Log($"[RadiusTargetFinder] Found {validTargets.Count()} valid targets after targeting mode filter");
            
            var result = validTargets.ToList();
            foreach (var target in result)
            {
                Debug.Log($"[RadiusTargetFinder] Valid target: {target.name} (tag: {target.tag})");
            }

            return result;
        }
    }

    public class SingleTargetFinder : ITargetFinder
    {
        private readonly GameObject target;

        public SingleTargetFinder(GameObject target)
        {
            this.target = target;
        }

        public string Description => $"Single(target={target})";

        public List<GameObject> ResolveTargets(GameObject source)
        {
            // If the target is alive/valid you could optionally check here:
            // if (_target == null || _target.GetComponent<IHealth>() == null) return new();
            return target != null
                ? new List<GameObject> { target }
                : new List<GameObject>();
        }
    }

    public class RandomSingleTargetFinder : ITargetFinder
    {
        private readonly TargetingMode mode;
        
        public RandomSingleTargetFinder(TargetingMode mode)
        {
            this.mode = mode;
        }

        public string Description => $"RandomSingle(mode={mode})";
        
        public List<GameObject> ResolveTargets(GameObject source)
        {
            var all = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                            .Where(go => go.GetComponent<IHealth>() != null && MatchesMode(source, go))
                            .ToList();
            if (all.Count == 0) return new List<GameObject>();
            int randomIndex = UnityEngine.Random.Range(0, all.Count);
            return new List<GameObject> { all[randomIndex] };
        }
        
        private bool MatchesMode(GameObject source, GameObject target)
        {
            if (source == target)
                return mode.HasFlag(TargetingMode.Self);
            bool sameTag = source.CompareTag(target.tag);
            if (sameTag && mode.HasFlag(TargetingMode.Allies)) return true;
            if (!sameTag && mode.HasFlag(TargetingMode.Enemies)) return true;
            return false;
        }
    }
}
