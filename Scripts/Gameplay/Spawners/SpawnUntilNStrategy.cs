using UnityEngine;

namespace VT.Gameplay.Spawners
{
    [System.Serializable]
    public class SpawnUntilNStrategy : SpawnStrategy
    {
        [Header("Config")]
        public int N = 5;
        public float Interval = 1f;

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

            if (context.ActiveCount >= N)
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

        public override int GetMaxActive() => N;
    }
}
