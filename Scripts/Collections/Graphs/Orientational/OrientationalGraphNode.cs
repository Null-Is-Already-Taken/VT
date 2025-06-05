using System.Collections.Generic;

namespace VT.Collections.Graphs.Orientational
{
    public class OrientationalGraphNode<T> : GraphNode<T>
    {
        public Dictionary<Direction, OrientationalGraphNode<T>> DirectedNeighbors { get; private set; }

        public OrientationalGraphNode(T value) : base(value)
        {
            DirectedNeighbors = new Dictionary<Direction, OrientationalGraphNode<T>>();
        }

        public void AddDirectedNeighbor(Direction dir, OrientationalGraphNode<T> neighbor)
        {
            if (neighbor != null && neighbor != this)
            {
                DirectedNeighbors[dir] = neighbor;
            }
        }

        public void RemoveDirectedNeighbor(Direction dir)
        {
            DirectedNeighbors.Remove(dir);
        }

        public bool TryGetNeighbor(Direction dir, out OrientationalGraphNode<T> neighbor)
        {
            return DirectedNeighbors.TryGetValue(dir, out neighbor);
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            result.Append($"Node: {Value} | Neighbors: ");

            foreach (var kvp in DirectedNeighbors)
            {
                result.Append($"{kvp.Key}→{kvp.Value.Value}, ");
            }

            if (DirectedNeighbors.Count > 0)
                result.Length -= 2; // Remove trailing comma and space

            return result.ToString();
        }
    }
}
