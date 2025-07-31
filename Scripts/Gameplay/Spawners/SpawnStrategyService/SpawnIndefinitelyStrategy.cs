using UnityEngine;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class SpawnIndefinitelyStrategy : SpawnStrategy
    {
        [Tooltip("Optional soft-cap. Set = 0 to ignore and let Spawner's own check handle it.")]
        public int SoftMaxActive = 0;

        public override int GetMaxActive()
        {
            SoftMaxActive = Mathf.Max(0, SoftMaxActive);

            // if no soft cap, effectively infinite
            return SoftMaxActive > 0 ? SoftMaxActive : int.MaxValue;
        }
    }
}
