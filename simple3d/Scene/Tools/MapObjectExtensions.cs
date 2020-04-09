using System;
using System.Runtime.CompilerServices;

namespace simple3d.Scene.Tools
{
    public static class MapObjectExtensions
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