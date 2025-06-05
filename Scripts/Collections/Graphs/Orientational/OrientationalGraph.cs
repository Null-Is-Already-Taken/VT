using System.Collections.Generic;

namespace VT.Collections.Graphs.Orientational
{
    public class OrientationalGraph<T> : Graph<T>
    {
        public override GraphNode<T> GetOrCreateNode(T value)
        {
            if (!nodeLookup.TryGetValue(value, out var node))
            {
                node = new OrientationalGraphNode<T>(value);
                nodeLookup[value] = node;
            }

            return node;
        }

        public void AddDirectedEdge(T from, T to, Direction direction, bool bidirectional = true)
        {
            var fromNode = (OrientationalGraphNode<T>)GetOrCreateNode(from);
            var toNode = (OrientationalGraphNode<T>)GetOrCreateNode(to);

            fromNode.AddDirectedNeighbor(direction, toNode);

            if (bidirectional)
            {
                var opposite = GetOpposite(direction);
                toNode.AddDirectedNeighbor(opposite, fromNode);
            }
        }

        public void RemoveDirectedEdge(T from, Direction direction, bool bidirectional = true)
        {
            if (nodeLookup.TryGetValue(from, out var fromBaseNode) &&
                fromBaseNode is OrientationalGraphNode<T> fromNode)
            {
                if (fromNode.TryGetNeighbor(direction, out var toNode))
                {
                    fromNode.RemoveDirectedNeighbor(direction);

                    if (bidirectional)
                    {
                        var opposite = GetOpposite(direction);
                        toNode.RemoveDirectedNeighbor(opposite);
                    }
                }
            }
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("OrientationalGraph:");

            foreach (var node in GetAllNodes())
            {
                result.AppendLine(node.ToString());
            }

            return result.ToString();
        }

        private Direction GetOpposite(Direction dir)
        {
            return dir switch
            {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Top => Direction.Bottom,
                Direction.Bottom => Direction.Top,
                _ => dir
            };
        }

        public new IEnumerable<OrientationalGraphNode<T>> GetAllNodes()
        {
            foreach (var node in nodeLookup.Values)
            {
                yield return (OrientationalGraphNode<T>)node;
            }
        }
    }
}
