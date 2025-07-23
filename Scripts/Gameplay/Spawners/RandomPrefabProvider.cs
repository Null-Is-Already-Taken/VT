using UnityEngine;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class RandomPrefabProvider : PrefabProvider
    {
        private readonly System.Random rng = new();

        public override GameObject QueryNextPrefab()
        {
            if (Prefabs == null || Prefabs.Count == 0) return null;
            int idx = rng.Next(Prefabs.Count);
            return Prefabs[idx];
        }
    }
} 