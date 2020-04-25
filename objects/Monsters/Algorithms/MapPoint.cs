﻿namespace objects.Monsters.Algorithms
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
    }
}