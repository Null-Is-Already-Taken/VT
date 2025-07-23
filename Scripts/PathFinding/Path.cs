using System.Collections.Generic;
using VT.Collections.Graphs.Orientational;

namespace VT.PathFinding
{
    /// <summary>
    /// Represents a path in the graph with metadata like turn count.
    /// </summary>
    public class Path<T>
    {
        public List<OrientationalGraphNode<T>> Nodes { get; }
        public int TurnCount { get; }
        public bool IsValid => Nodes != null && Nodes.Count > 0;

        public Path(List<OrientationalGraphNode<T>> nodes, int turnCount)
        {
            Nodes = nodes;
            TurnCount = turnCount;
        }

        public override string ToString()
        {
            return $"Path (Turns: {TurnCount}, Length: {Nodes?.Count ?? 0})";
        }
    }
}