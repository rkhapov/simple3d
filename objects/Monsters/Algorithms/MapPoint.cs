using System.Numerics;

namespace objects.Monsters.Algorithms
{
    public class MapPoint
    {
        public MapPoint(int y, int x)
        {
            Y = y;
            X = x;
        }
        
        public readonly int X;
        public readonly int Y;

        public static MapPoint FromVector2(Vector2 source)
        {
            return new MapPoint((int) source.Y, (int) source.X);
        }

        public override int GetHashCode()
        {
            return unchecked(X * 31337 ^ Y);
        }

        public override string ToString()
        {
            return $"({Y}; {X})";
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                null => false,
                MapPoint other => (X == other.X && Y == other.Y),
                _ => false
            };
        }

        public int GetManhattanDistanceTo(MapPoint goal)
        {
            //manhettan
            var dx = X - goal.X;
            if (dx < 0)
                dx = -dx;

            var dy = Y - goal.Y;
            if (dy < 0)
                dy = -dy;

            return dx + dy;
        }
    }
}