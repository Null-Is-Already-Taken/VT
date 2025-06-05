using System.Collections.Generic;

namespace VT.Collections.Graphs
{
    public class GraphNode<T>
    {
        public T Value { get; private set; }
        public HashSet<GraphNode<T>> Neighbors { get; private set; }

        public GraphNode(T value)
        {
            Value = value;
            Neighbors = new HashSet<GraphNode<T>>();
        }

        public virtual void AddNeighbor(GraphNode<T> neighbor)
        {
            if (neighbor != null && neighbor != this)
            {
                Neighbors.Add(neighbor);
            }
        }

        public virtual void RemoveNeighbor(GraphNode<T> neighbor)
        {
            if (neighbor != null)
            {
                Neighbors.Remove(neighbor);
            }
        }
    }
}
