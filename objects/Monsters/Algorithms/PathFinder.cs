using System;
using System.Collections.Generic;
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

                foreach (var (next, distance) in GetNeighbours(map, current))
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

            Console.WriteLine(iterationsCounter);
            if (!previous.ContainsKey(goal))
                return null;
            
            var path = new List<MapPoint>();
            var c = goal;

            while (c != null)
            {
                path.Add(c);
                c = previous[c];
            }

            return path;
        }

        private class MapPointComparer : IComparer<(MapPoint point, int priority)>
        {
            public int Compare((MapPoint point, int priority) x, (MapPoint point, int priority) y)
            {
                return x.priority.CompareTo(y.priority);
            }
        }

        private static int H(MapPoint v, MapPoint goal)
        {
            //chebushev
            var dx = v.X - goal.X;
            if (dx < 0)
                dx = -dx;

            var dy = v.Y - goal.Y;
            if (dy < 0)
                dy = -dy;

            return dx + dy;
        }

        private static IEnumerable<(MapPoint point, int distance)> GetNeighbours(Map map, MapPoint p)
        {
            var mapHeight = map.Height;
            var mapWidth = map.Width;

            foreach (var offset in NeighboursOffsets)
            {
                var x = p.X + offset.X;
                var y = p.Y + offset.Y;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight || map.At(y, x).Type != MapCellType.Empty)
                    continue;
                var distance = (x == p.X ? 0 : 1) + (y == p.Y ? 0 : 1);

                yield return (new MapPoint(y, x), distance);
            }
        }

        private static readonly MapPoint[] NeighboursOffsets = new[]
        {
            new MapPoint(-1, -1),
            new MapPoint(-1, 0),
            new MapPoint(-1, +1),
            
            new MapPoint(+1, 0),
            new MapPoint(+1, -1),
            new MapPoint(+1, +1),

            new MapPoint(0, +1),
            new MapPoint(0, -1)
        };
    }
}