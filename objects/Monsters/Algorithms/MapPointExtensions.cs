using System.Collections.Generic;
using simple3d.Levels;

namespace objects.Monsters.Algorithms
{
    public static class MapPointExtensions
    {
        public static IEnumerable<MapPoint> GetPointsAtRadius(this MapPoint point, Map map, int manhattanRadius)
        {
            var q = new Queue<MapPoint>();
            var visited = new HashSet<MapPoint>();

            q.Enqueue(point);
            visited.Add(point);

            while (q.Count != 0)
            {
                var v = q.Dequeue();

                yield return v;

                foreach (var (neighbour, _) in v.GetNeighbours(map))
                {
                    if (visited.Contains(neighbour)
                        || neighbour.GetManhattanDistanceTo(point) > manhattanRadius)
                    {
                        continue;
                    }

                    visited.Add(neighbour);
                    q.Enqueue(neighbour);
                }
            }
        }
        
        public static IEnumerable<(MapPoint point, int distance)> GetNeighbours(this MapPoint p, Map map)
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

                yield return (new MapPoint(y, x), 1);
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