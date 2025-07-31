using UnityEngine;

namespace VT.Gameplay.Spawners
{
    public abstract class SpawnStrategy
    {
        [Tooltip("Seconds between spawns.")]
        public float Interval = 1f;

        protected ISpawnerContext context;
        private float timer = 0f;

        public virtual void Initialize(ISpawnerContext context)
        {
            this.context = context;
            timer = 0f;
        }

        public virtual void Update(float deltaTime)
        {
            timer += deltaTime;
        }

        public virtual void Reset()
        {
            timer = 0f;
        }

        public virtual bool ShouldSpawn()
        {
            if (context == null)
                return false;

            // enforce strategy-specific cap
            if (context.ActiveCount >= GetMaxActive())
                return false;

            if (timer >= Interval)
            {
                timer = 0f;
                return true;
            }

            return false;
        }

        /// <summary>
        /// How many can be active before we stop spawning?
        /// </summary>
        public abstract int GetMaxActive();
    }
}
