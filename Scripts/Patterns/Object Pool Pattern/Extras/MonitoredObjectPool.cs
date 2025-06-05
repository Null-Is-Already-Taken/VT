using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VT.Patterns.ObjectPoolPattern.Extras
{
    public class MonitoredObjectPool<T> : ObjectPool<T> where T : Component
    {
        public MonitoredObjectPool(
            T prefab,
            Transform parent,
            MonoBehaviour monitorHost,
            int historyLength = 5,
            float monitorInterval = 60f
        ) : base(prefab, parent)
        {
            this.historyLength = historyLength;
            this.monitorInterval = monitorInterval;

            if (monitorHost != null)
            {
                monitoringCoroutine = monitorHost.StartCoroutine(MonitorUsage());
            }
        }

        protected override void OnObjectGet(T item)
        {
            base.OnObjectGet(item);
            currentUsageThisInterval++;
        }

        private IEnumerator MonitorUsage()
        {
            while (true)
            {
                yield return new WaitForSeconds(monitorInterval);

                usageHistory.Enqueue(currentUsageThisInterval);
                if (usageHistory.Count > historyLength)
                    usageHistory.Dequeue();

                int maxUsed = usageHistory.Count > 0 ? usageHistory.Max() : 0;
                int excess = Count - maxUsed;

                for (int i = 0; i < excess; i++)
                {
                    if (TryShrink(out var item))
                    {
                        Object.Destroy(item.gameObject);
                    }
                }

                currentUsageThisInterval = 0;
            }
        }

        private bool TryShrink(out T item)
        {
            if (Count > 0)
            {
                item = pool.Pop(); // using base field directly
                return true;
            }

            item = null;
            return false;
        }

        private readonly Queue<int> usageHistory = new();
        private int currentUsageThisInterval = 0;
        private readonly int historyLength;
        private readonly float monitorInterval;
        private Coroutine monitoringCoroutine;
    }
}
