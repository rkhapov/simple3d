using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace simple3d.MathUtils
{
    public static class Vector2Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this ref Vector2 vector2, float angle)
        {
            var newX = vector2.X * MathF.Cos(angle) - vector2.Y * MathF.Sin(angle);
            var newY = vector2.X * MathF.Sin(angle) + vector2.Y * MathF.Cos(angle);

            vector2.X = newX;
            vector2.Y = newY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OnSegment(this Vector2 vector, Vector2 begin, Vector2 end)
        {
            var max = Vector2.Max(begin, end);
            var min = Vector2.Min(begin, end);

            return min.X < vector.X && vector.X < max.X &&
                   min.Y < vector.Y && vector.Y < max.Y;
        }

        public static PointToLinePosition GetPositionToLine(this Vector2 point, Vector2 p0, Vector2 p1)
        {
            var a = p1 - p0;
            var b = point - p0;
            var sa = a.X * b.Y - b.X * a.Y;

            if (sa < -1e-6f)
            {
                return PointToLinePosition.Right;
            }

            if (sa > 1e-6f)
            {
                return PointToLinePosition.Left;
            }

            if ((a.X * b.X < 0.0) || (a.Y * b.Y < 0.0))
                return PointToLinePosition.Behind;

            if (a.LengthSquared() < b.LengthSquared())
                return PointToLinePosition.Beyond;

            //TODO: fix comparison
            if (p0 == point)
                return PointToLinePosition.Origin;

            if (p1 == point)
                return PointToLinePosition.Destination;

            return PointToLinePosition.Between;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InTriangle(this Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            return point.GetPositionToLine(a, b) != PointToLinePosition.Left
                   && point.GetPositionToLine(b, c) != PointToLinePosition.Left
                   && point.GetPositionToLine(c, a) != PointToLinePosition.Left;
        }
    }
}