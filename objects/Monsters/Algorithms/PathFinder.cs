using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using simple3d.Levels;

namespace objects.Monsters.Algorithms
{
    public class PathFinder
    {
        public static List<MapPoint> FindPath(Map map, MapPoint start, MapPoint goal, int maxIterations=1000)
        {
            //A star
            var cost = new Dictionary<MapPoint, int>();
            var frontier = new MinHeap<(MapPoint point, int priority)>(new MapPointComparer());
            var previous = new Dictionary<MapPoint, MapPoint>();

            frontier.Add((start, 0));
            cost[start] = 0;
            previous[start] = null;

            var iterationsCounter = 0;

            while (frontier.Count != 0 && iterationsCounter < maxIterations)
            {
                iterationsCounter++;
                var (current, _) = frontier.ExtractDominating();

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var (next, distance) in current.GetNeighbours(map))
                {
                    var newCost = distance + cost[current];

                    if (!cost.TryGetValue(next, out var oldCost) || oldCost > newCost)
                    {
                        cost[next] = newCost;
                        frontier.Add((next, newCost + H(next, goal)));
                        previous[next] = current;
                    }
                }
            }

            if (!previous.ContainsKey(goal))
                return null;
            
            var path = new List<MapPoint>();
            var c = goal;

            while (c != null)
            {
                path.Add(c);
                c = previous[c];
            }

            if (path.Count == 1)
                path.Add(start);

            return path;
        }

        private class MapPointComparer : IComparer<(MapPoint point, int priority)>
        {
            public int Compare((MapPoint point, int priority) x, (MapPoint point, int priority) y)
            {
                return x.priority.CompareTo(y.priority);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int H(MapPoint v, MapPoint goal)
        {
            return v.GetManhattanDistanceTo(goal);
        }
    }
}