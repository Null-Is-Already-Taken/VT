using UnityEngine;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class SequentialPrefabProvider : PrefabProvider
    {
        private int currentIndex = 0;

        public override GameObject QueryNextPrefab()
        {
            if (Prefabs == null || Prefabs.Count == 0) return null;
            var prefab = Prefabs[currentIndex];
            currentIndex = (currentIndex + 1) % Prefabs.Count;
            return prefab;
        }
    }
} 