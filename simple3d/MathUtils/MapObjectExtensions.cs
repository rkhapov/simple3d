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
    }
}