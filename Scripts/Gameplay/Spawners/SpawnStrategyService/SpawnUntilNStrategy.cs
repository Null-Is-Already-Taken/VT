using UnityEngine;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class SpawnUntilNStrategy : SpawnStrategy
    {
        [Tooltip("Maximum number of simultaneous spawns.")]
        public int N = 5;

        public override int GetMaxActive() => N;
    }
}
