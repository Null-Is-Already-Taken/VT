using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VT.Extensions;

namespace VT.Collections.Wrapper
{
    public abstract class ListWrapper<T> : IEnumerable<T>
    {
        public List<T> list = new();
        public int Count => list?.Count ?? 0;
        public void Clear() => list?.Clear();
        public bool IsNullOrEmpty() => list.IsNullOrEmpty();

        public void Add(T entry) => list.Add(entry);
        public bool Remove(T entry) => list.Remove(entry);
        public bool Contains(T entry) => list.Contains(entry);

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        // IEnumerable<T> implementation
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        // Non-generic IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
