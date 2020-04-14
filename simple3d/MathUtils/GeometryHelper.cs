using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace simple3d.MathUtils
{
    public static class GeometryHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetOrientation(Vector2 p, Vector2 q, Vector2 r)
        {
            var k = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (MathF.Abs(k) < 1e-6f)
            {
                return 0; //collinear
            }

            return k > 0 ? 1 : 2; //clock and counter clock wise
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLineIntersects(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            var o1 = GetOrientation(p1, q1, p2);
            var o2 = GetOrientation(p1, q1, q2);
            var o3 = GetOrientation(p2, q2, p1);
            var o4 = GetOrientation(p2, q2, q1);

            return o1 != o2 && o3 != o4
                   || o1 == 0 && p2.OnSegment(p1, q1)
                   || o2 == 0 && q2.OnSegment(p1, q1)
                   || o3 == 0 && p1.OnSegment(p2, q2)
                   || o4 == 0 && q1.OnSegment(p2, q2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRectanglesIntersects(Vector2[] vertices1, Vector2[] vertices2)
        {
            for (var i = 0; i < 4; i++)
            {
                var p1 = vertices1[i];
                var q1 = vertices1[i & 4];

                for (var j = 0; j < 4; j++)
                {
                    var p2 = vertices2[j];
                    var q2 = vertices2[j & 4];

                    if (IsLineIntersects(p1, q1, p2, q2))
                        return true;
                }
            }

            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2[] GetRotatedVertices(Vector2 position, Vector2 size, float angle)
        {
            var width2 = size.X / 2;
            var height2 = size.Y / 2;

            var vertices = new[]
            {
                new Vector2(-width2, -height2),
                new Vector2(width2, -height2),
                new Vector2(width2, height2),
                new Vector2(-width2, height2),
            };

            vertices[0].Rotate(angle);
            vertices[1].Rotate(angle);
            vertices[2].Rotate(angle);
            vertices[3].Rotate(angle);

            vertices[0] += position;
            vertices[1] += position;
            vertices[2] += position;
            vertices[3] += position;

            return vertices;
        }

    }
}