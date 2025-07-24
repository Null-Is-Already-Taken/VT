using System;
using System.Collections;
using UnityEngine;

namespace VT.Gameplay.Spawners
{
    public class Spawner : MonoBehaviour, ISpawnerContext
    {
        [Header("Providers / Strategy")]
        [SerializeReference] private SpawnStrategy spawnStrategy;
        [SerializeReference] private LocationProvider locationProvider;
        [SerializeReference] private PrefabProvider prefabProvider;

        [Header("Debug")]
        [SerializeField] private bool debugLogs = false;

        public event Action<GameObject> OnSpawn;

        public int ActiveCount { get; private set; }

        private Coroutine spawnRoutine;
        private bool isSpawning;

        private void OnEnable()
        {
            if (spawnStrategy != null && locationProvider != null && prefabProvider != null)
            {
                prefabProvider.Initialize();
                spawnStrategy.Initialize(this);
                Log("Initialized with default providers/strategy.");
                StartSpawning();
            }
            else
            {
                LogWarning("Missing one or more providers/strategy. Spawning will not auto-start.");
            }
        }

        private void OnDisable()
        {
            StopSpawning();
            spawnStrategy?.Reset();
            Log("Spawner disabled. Stopped spawning and reset strategy.");
        }

        private void OnDestroy()
        {
            // Clean up any remaining active objects
            if (prefabProvider != null)
            {
                // Note: This would require tracking active objects
                // For now, rely on individual NotifyDespawn calls
            }
        }

        public void StartSpawning()
        {
            if (isSpawning) { Log("StartSpawning called but already spawning."); return; }

            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnLoop());
            Log("Spawn loop started.");
        }

        public void StopSpawning()
        {
            if (!isSpawning) { Log("StopSpawning called but not spawning."); return; }

            isSpawning = false;
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            Log("Spawn loop stopped.");
        }

        public void NotifyDespawn(GameObject obj)
        {
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
            prefabProvider?.ReturnPrefab(obj); // Return to pool
            Log($"Despawned: {obj.name}. ActiveCount={ActiveCount}");
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                if (!isSpawning) yield break;

                if (spawnStrategy == null || locationProvider == null || prefabProvider == null)
                {
                    LogWarning("Spawner tick skipped: missing provider/strategy.");
                    yield return null;
                    continue;
                }

                float dt = Time.deltaTime;
                spawnStrategy.Update(dt);

                if (spawnStrategy.ShouldSpawn())
                {
                    Vector3 position = locationProvider.GetSpawnLocation();
                    Log($"Spawn location: {position}");

                    GameObject obj = prefabProvider.GetPrefab();
                    if (obj != null)
                    {
                        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
                        ActiveCount++;
                        Log($"Spawned: {obj.name} at {position}. ActiveCount={ActiveCount}");
                        OnSpawn?.Invoke(obj);
                    }
                    else
                    {
                        LogWarning("PrefabProvider returned null prefab.");
                    }
                }

                LogVerbose($"Spawn loop tick: ActiveCount={ActiveCount}, dt={dt}");

                yield return null;
            }
        }

        public void SwitchStrategy(SpawnStrategy strategy)
        {
            spawnStrategy = strategy;
            spawnStrategy.Initialize(this);
            Log($"Switched strategy to {strategy?.GetType().Name}");
        }

        public void SwitchLocationProvider(LocationProvider provider)
        {
            locationProvider = provider;
            Log($"Switched location provider to {provider?.GetType().Name}");
        }

        public void SwitchPrefabProvider(PrefabProvider provider)
        {
            prefabProvider = provider;
            Log($"Switched prefab provider to {provider?.GetType().Name}");
        }

        // --- Debug Utilities -------------------------------------------------

        private void Log(string msg)
        {
            if (debugLogs) Debug.Log($"[Spawner] {msg}", this);
        }

        private void LogWarning(string msg)
        {
            if (debugLogs) Debug.LogWarning($"[Spawner] {msg}", this);
        }

        private void LogError(string msg)
        {
            if (debugLogs) Debug.LogError($"[Spawner] {msg}", this);
        }

        // Extra-verbose (spammy) info you can toggle in code quickly
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void LogVerbose(string msg)
        {
            if (debugLogs) Debug.Log($"[Spawner:Verbose] {msg}", this);
        }
    }
}
