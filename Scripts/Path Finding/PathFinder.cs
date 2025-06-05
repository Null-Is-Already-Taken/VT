using System;
using VT.Collections.Graphs.Orientational;

namespace VT.PathFinding
{
    public static class PathFinder
    {
        public static Path<T> FindPath<T>(
            OrientationalGraph<T> graph,
            T start,
            T goal,
            Func<T, T, bool> isGoal,
            Func<T, bool> isPassable,
            IPathFindingStrategy<T> strategy)
        {
            return strategy.FindPath(graph, start, goal, isGoal, isPassable);
        }
    }
}
