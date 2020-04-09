using System;
using System.Runtime.CompilerServices;

namespace simple3d.Levels.Tools
{
    internal static class MapObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float GetDistanceToPlayer(this IMapObject mapObject, PlayerCamera camera)
        {
            var dx = mapObject.PositionX - camera.X;
            var dy = mapObject.PositionY - camera.Y;

            return MathF.Sqrt(dx * dx + dy * dy);
        }
    }
}