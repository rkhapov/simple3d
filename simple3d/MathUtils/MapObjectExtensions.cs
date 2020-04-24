using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using simple3d.Levels;

namespace simple3d.MathUtils
{
    public static class MapObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2[] GetRotatedVertices(this IMapObject mapObject)
        {
            return GeometryHelper.GetRotatedVertices(
                mapObject.Position,
                mapObject.Size,
                mapObject.DirectionAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZeroSized(this IMapObject mapObject)
        {
            return mapObject.Size.LengthSquared() < 1e-6f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRadius(this IMapObject mapObject, Vector2 point)
        {
            var max = MathF.Max(mapObject.Size.X, mapObject.Size.Y);

            return (mapObject.Position - point).LengthSquared() < max * max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IMapObject mapObject, Vector2 point)
        {
            return GeometryHelper.IsPointAtRectangle(point, mapObject.GetRotatedVertices());
        }

        public static float GetAngleToPlayer(this IMapObject mapObject, Player player)
        {
            const float pi2 = MathF.PI * 0.5f;
            var dv = mapObject.Position - player.Position;
            var angle = player.DirectionAngle - MathF.Atan2(dv.Y, dv.X);

            if (angle < -MathF.PI)
            {
                angle += pi2;
            }

            if (angle > MathF.PI)
            {
                angle -= pi2;
            }

            return angle;
        }
    }
}