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

        [Header("Debug")]
        public bool debugLogs   = false;
        public bool verboseTick = false; // super chatty per-Update logs

        private ISpawnerContext context;
        private float timer = 0f;

        public override void Initialize(ISpawnerContext context)
        {
            this.context = context;
            timer = 0f;
            Log($"Initialized. Interval={Interval}s, SoftMaxActive={(SoftMaxActive <= 0 ? "∞" : SoftMaxActive.ToString())}");
        }

        public override void Update(float deltaTime)
        {
            timer += deltaTime;
            LogVerbose($"Update dt={deltaTime:F3}, timer={timer:F3}, active={context?.ActiveCount}");
        }

        public override void Reset()
        {
            timer = 0f;
            Log("Reset timer to 0.");
        }

        public override bool ShouldSpawn()
        {
            if (context == null)
            {
                LogWarning("Context is null. Cannot spawn.");
                return false;
            }

            // If you want an optional soft cap, enforce it here:
            if (SoftMaxActive > 0 && context.ActiveCount >= SoftMaxActive)
            {
                LogVerbose($"Blocked by SoftMaxActive: {context.ActiveCount}/{SoftMaxActive}");
                return false;
            }

            if (timer >= Interval)
            {
                timer = 0f;
                Log($"ShouldSpawn = TRUE (active={context.ActiveCount})");
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

        // --------- Debug helpers ----------
        private void Log(string msg)
        {
            if (debugLogs) UnityEngine.Debug.Log($"[SpawnIndefinitelyStrategy] {msg}");
        }

        private void LogWarning(string msg)
        {
            if (debugLogs) UnityEngine.Debug.LogWarning($"[SpawnIndefinitelyStrategy] {msg}");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void LogVerbose(string msg)
        {
            if (debugLogs && verboseTick)
                UnityEngine.Debug.Log($"[SpawnIndefinitelyStrategy:Verbose] {msg}");
        }
    }
}
