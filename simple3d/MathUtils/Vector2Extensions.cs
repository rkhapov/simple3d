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
    }
}