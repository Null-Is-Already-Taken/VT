using System;
using System.Collections.Generic;
using UnityEngine;
using VT.Collections.Graphs.Orientational;

namespace VT.PathFinding.BFS
{
    public class BFSPathFinder<T> : IPathFindingStrategy<T>
    {
        public Path<T> FindPath(OrientationalGraph<T> graph, T start, T goal, Func<T, T, bool> isGoal, Func<T, bool> isPassable)
        {
            var startNode = (OrientationalGraphNode<T>)graph.GetOrCreateNode(start);
            int turnCount = 0;

            var queue = new Queue<SearchState<T>>();
            var visited = new Dictionary<(OrientationalGraphNode<T>, Direction), int>();
            var searchState = new SearchState<T>(startNode, Direction.None, turnCount, new() { startNode });

            queue.Enqueue(searchState);
            Debug.Log($"Enqueued start node: {start}");

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                Debug.Log($"Dequeued node: {current.Node.Value}, path so far: [{string.Join(" -> ", current.PathSoFar)}], turn count: {current.TurnCount}, prev dir: {current.PrevDirection}");

                foreach (var (direction, neighbor) in current.Node.DirectedNeighbors)
                {
                    Debug.Log($"  Checking neighbor: {neighbor.Value} in direction {direction}");

                    if (!isPassable(neighbor.Value))
                    {
                        Debug.Log($"    Skipped (not passable): {neighbor.Value}");
                        continue;
                    }

                    bool isTurn = direction != current.PrevDirection;
                    int newTurnCount = current.TurnCount + (isTurn ? 1 : 0);
                    var key = (neighbor, direction);

                    if (!visited.TryGetValue(key, out int bestTurnCount) || newTurnCount < bestTurnCount)
                    {
                        visited[key] = newTurnCount;
                        var newSearchState = new SearchState<T>(neighbor, direction, newTurnCount, new List<OrientationalGraphNode<T>>(current.PathSoFar));
                        newSearchState.PathSoFar.Add(neighbor);

                        // if (EqualityComparer<T>.Default.Equals(neighbor.Value, goal))
                        if (isGoal(neighbor.Value, goal))
                        {
                            Debug.Log($"    Goal found! Path: [{string.Join(" -> ", newSearchState.PathSoFar)}], total turns: {newTurnCount}");
                            return new Path<T>(newSearchState.PathSoFar, newTurnCount);
                        }

                        queue.Enqueue(newSearchState);
                    }
                    else
                    {
                        Debug.Log($"    Skipped (already visited with fewer turns): {neighbor.Value}");
                    }
                }
            }

            Debug.Log("No path found.");
            return null;
        }
    }
}
