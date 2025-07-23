using System;
using VT.Collections.Graphs.Orientational;

namespace VT.PathFinding
{
    public interface IPathFindingStrategy<T>
    {
        Path<T> FindPath(OrientationalGraph<T> graph, T start, T goal, Func<T, T, bool> isGoal, Func<T, bool> isPassable);
    }
}