using System.Runtime.CompilerServices;

namespace simple3d.Levels.Tools
{
    internal static class MapObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDistanceToPlayerSquared(this IMapObject mapObject, Player player)
        {
            return (mapObject.Position - player.Position).LengthSquared();
        }
    }
}