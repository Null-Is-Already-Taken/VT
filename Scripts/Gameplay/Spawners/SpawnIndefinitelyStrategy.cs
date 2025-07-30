using UnityEngine;

namespace VT.Gameplay.Spawners
{
    /// <summary>
    /// Spawns forever at a fixed interval. MaxActive is effectively unlimited.
    /// </summary>
    [System.Serializable]
    public class SpawnIndefinitelyStrategy : SpawnStrategy
    {
        [Header("Config")]
        [Tooltip("Seconds between spawns.")]
        public float Interval = 1f;

        [Tooltip("Optional soft-cap. Set <= 0 to ignore and let Spawner's own check handle it.")]
        public int SoftMaxActive = 0;

        private ISpawnerContext context;
        private float timer = 0f;

        public override void Initialize(ISpawnerContext context)
        {
            this.context = context;
            timer = 0f;
        }

        public override void Update(float deltaTime)
        {
            timer += deltaTime;
        }

        public override void Reset()
        {
            timer = 0f;
        }

        public override bool ShouldSpawn()
        {
            if (context == null)
            {
                return false;
            }

            // If you want an optional soft cap, enforce it here:
            if (SoftMaxActive > 0 && context.ActiveCount >= SoftMaxActive)
            {
                return false;
            }

            if (timer >= Interval)
            {
                timer = 0f;
                return true;
            }

            return false;
        }

        public override int GetMaxActive()
        {
            // Return huge number so Spawner's check won't block.
            // If you rely solely on SoftMaxActive, keep this int.MaxValue.
            return int.MaxValue;
        }
    }
}
