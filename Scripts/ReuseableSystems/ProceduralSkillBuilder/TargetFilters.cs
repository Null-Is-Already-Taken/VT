using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public static class TargetFilters
    {
        public static bool ComponentFilter<T>(GameObject target)
        {
            bool hasComponent = target != null && target.GetComponent<T>() != null;
            if (!hasComponent && target != null)
            {
                Debug.Log($"[TargetFilters] Target {target.name} does not have {typeof(T).Name} component");
            }
            return hasComponent;
        }

        public static bool WithinRadiusFilter(GameObject source, GameObject target, float radius)
        {
            if (source == null || target == null) return false;
            
            float distance = Vector3.Distance(source.transform.position, target.transform.position);
            bool withinRadius = distance <= radius;
            
            if (!withinRadius)
            {
                Debug.Log($"[TargetFilters] Target {target.name} is outside radius. Distance: {distance}, Radius: {radius}");
            }
            
            return withinRadius;
        }

        public static bool TargetingModeFilter(GameObject source, GameObject target, TargetingMode targetingMode)
        {
            if (source == null || target == null) return false;
            
            bool isSelf = target == source;
            bool sameTag = source.CompareTag(target.tag);
            
            Debug.Log($"[TargetFilters] Checking targeting mode for {target.name}. IsSelf: {isSelf}, SameTag: {sameTag}, Mode: {targetingMode}");
            
            if (isSelf)
            {
                bool allowed = targetingMode.HasFlag(TargetingMode.Self);
                Debug.Log($"[TargetFilters] Self targeting allowed: {allowed}");
                return allowed;
            }
            else
            {
                if (sameTag)
                {
                    bool allowed = targetingMode.HasFlag(TargetingMode.Allies);
                    Debug.Log($"[TargetFilters] Ally targeting allowed: {allowed}");
                    return allowed;
                }
                else
                {
                    bool allowed = targetingMode.HasFlag(TargetingMode.Enemies);
                    Debug.Log($"[TargetFilters] Enemy targeting allowed: {allowed}");
                    return allowed;
                }
            }
        }
    }
}
