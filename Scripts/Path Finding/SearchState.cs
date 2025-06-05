using System.Collections.Generic;
using VT.Collections.Graphs.Orientational;

namespace VT.PathFinding
{
    public class SearchState<T>
    {
        public OrientationalGraphNode<T> Node { get; }
        public Direction PrevDirection { get; }
        public int TurnCount { get; }
        public List<OrientationalGraphNode<T>> PathSoFar { get; }

        public SearchState(OrientationalGraphNode<T> node, Direction prevDirection, int turnCount, List<OrientationalGraphNode<T>> pathSoFar)
        {
            Node = node;
            PrevDirection = prevDirection;
            TurnCount = turnCount;
            PathSoFar = new List<OrientationalGraphNode<T>>(pathSoFar);
        }
    }
}
