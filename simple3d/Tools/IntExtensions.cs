using System.Runtime.CompilerServices;

namespace simple3d.Tools
{
    public static class IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransparentColor(this int color)
        {
            //TODO: fix alpha channels at screen?
            return (color & 0xFF000000) == 0;
        }
    }
}