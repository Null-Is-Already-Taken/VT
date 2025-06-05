using System.Collections.Generic;
using UnityEngine;

namespace VT.Collections.Generic
{
    public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<KeyValueData> keyValueData = new();

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var item in keyValueData)
            {
                this[item.Key] = item.Value;
            }
        }

        public void OnBeforeSerialize()
        {
            keyValueData.Clear();
            foreach (var kvp in this)
            {
                keyValueData.Add(new() { Key = kvp.Key, Value = kvp.Value });
            }
        }

        [System.Serializable]
        private struct KeyValueData
        {
            public TKey Key;
            public TValue Value;
        }
    }
}
