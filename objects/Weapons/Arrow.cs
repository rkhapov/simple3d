using System;
using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace objects.Weapons
{
    public class Arrow : IMapObject
    {
        public Arrow(Vector2 position, Sprite sprite, float directionAngle)
        {
            Position = position;
            Sprite = sprite;
            DirectionAngle = directionAngle;
        }

        public Vector2 Position { get; private set; }
        public Vector2 Size { get; } = Vector2.One;
        public float DirectionAngle { get; }
        public Sprite Sprite { get; }

        public void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            var xRayUnit = MathF.Sin(DirectionAngle);
            var yRayUnit = MathF.Cos(DirectionAngle);
            var dx = xRayUnit * 0.02f * elapsedMilliseconds;
            var dy = yRayUnit * 0.02f * elapsedMilliseconds;
            var newPosition = Position + new Vector2(dx, dy);
            var testX = (int) newPosition.X;
            var testY = (int) newPosition.Y;
            var hit = !scene.Map.InBound(testY, testX) || scene.Map.At(testY, testX).Type != MapCellType.Empty;

            var vertices = this.GetRotatedVertices();
            foreach (var obj in scene.Objects)
            {
                if (obj is Arrow)
                    continue;

                if (!GeometryHelper.IsRectanglesIntersects(vertices, obj.GetRotatedVertices()))
                    continue;

                obj.OnShoot(scene, 42);
                hit = true;
                break;
            }

            if (hit)
            {
                scene.RemoveObject(this);
                return;
            }

            Position = newPosition;
        }

        public void OnLeftMeleeAttack(Scene scene, int damage)
        {
        }

        public void OnRightMeleeAttack(Scene scene, int damage)
        {
        }

        public void OnShoot(Scene scene, int damage)
        {
        }
    }
}