using UnityEngine;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class SpawnUntilNStrategy : SpawnStrategy
    {
        [Header("Config")]
        public int N = 5;
        public float Interval = 1f;

        [Header("Debug")]
        public bool debugLogs = false;
        public bool verboseTick = false; // super chatty per-Update logs

        private ISpawnerContext context;
        private float timer = 0f;

        public override void Initialize(ISpawnerContext context)
        {
            this.context = context;
            timer = 0f;
            Log($"Initialized. N={N}, Interval={Interval}s");
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

            if (context.ActiveCount >= N)
            {
                LogVerbose($"Blocked: ActiveCount {context.ActiveCount} >= N {N}");
                return false;
            }

            if (timer >= Interval)
            {
                timer = 0f;
                Log($"ShouldSpawn = TRUE (active={context.ActiveCount}/{N})");
                return true;
            }

            return false;
        }

        public override int GetMaxActive() => N;

        // --------- Debug helpers ----------
        private void Log(string msg)
        {
            if (debugLogs) UnityEngine.Debug.Log($"[SpawnUntilNStrategy] {msg}");
        }

        private void LogWarning(string msg)
        {
            if (debugLogs) UnityEngine.Debug.LogWarning($"[SpawnUntilNStrategy] {msg}");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void LogVerbose(string msg)
        {
            if (debugLogs && verboseTick)
                UnityEngine.Debug.Log($"[SpawnUntilNStrategy:Verbose] {msg}");
        }
    }
}
