using System.Collections.Generic;

namespace VT.Collections.Graphs
{
    public class Graph<T>
    {
        protected readonly Dictionary<T, GraphNode<T>> nodeLookup = new();

        public virtual GraphNode<T> GetOrCreateNode(T value)
        {
            if (!nodeLookup.TryGetValue(value, out var node))
            {
                node = new GraphNode<T>(value);
                nodeLookup[value] = node;
            }

            return node;
        }

        public virtual void AddEdge(T from, T to)
        {
            var fromNode = GetOrCreateNode(from);
            var toNode = GetOrCreateNode(to);
            fromNode.AddNeighbor(toNode);
            toNode.AddNeighbor(fromNode); // Bidirectional by default
        }

        public virtual void RemoveEdge(T from, T to)
        {
            if (nodeLookup.TryGetValue(from, out var fromNode) &&
                nodeLookup.TryGetValue(to, out var toNode))
            {
                fromNode.RemoveNeighbor(toNode);
                toNode.RemoveNeighbor(fromNode);
            }
        }

        public IEnumerable<GraphNode<T>> GetAllNodes() => nodeLookup.Values;
    }
}
