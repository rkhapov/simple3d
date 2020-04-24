using System;
using System.Linq;
using System.Numerics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace objects.Monsters
{
    public class Ghost : BaseAnimatedStaticMapObject
    {
        public Ghost(Vector2 position, Vector2 size, float directionAngle, Animation animation) : base(position, size, directionAngle, animation)
        {
        }

        private const float MoveSpeed = 0.001f;

        public override void OnShoot(Scene scene, ShootingWeapon weapon)
        {
            Console.WriteLine($"{this} has been hit");
            scene.RemoveObject(this);
        }

        public override void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
            scene.RemoveObject(this);
            base.OnLeftMeleeAttack(scene, weapon);
        }

        public override void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
            scene.RemoveObject(this);
            base.OnRightMeleeAttack(scene, weapon);
        }

        private void Move(Scene scene, float elapsedTime)
        {
            var map = scene.Map;
            var dx = (float) Math.Cos(DirectionAngle) * MoveSpeed * elapsedTime;
            var dy = (float) Math.Sin(DirectionAngle) * MoveSpeed * elapsedTime;
            var newPosition = Position + new Vector2(dx, 0);
            
            TryMove(scene, newPosition, map);
            newPosition = Position + new Vector2(0, dy);
            TryMove(scene, newPosition, map);
        }
        
        private void TryMove(Scene scene, Vector2 newPosition, Map map)
        {
            var ghostNewVertices = GeometryHelper.GetRotatedVertices(newPosition, Size, DirectionAngle);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var vertex in ghostNewVertices)
            {
                var testX = (int) vertex.X;
                var testY = (int) vertex.Y;

                if (!map.InBound(testY, testX) || !CellTypes.walkable.Contains(map.At(testY, testX).Type))
                {
                    return;
                }
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var objectVertices in scene
                .Objects
                .Where(obj => obj != this)
                .Select(o => o.GetRotatedVertices()))
            {
                if (GeometryHelper.IsRectanglesIntersects(ghostNewVertices, objectVertices))
                {
                    return;
                }
            }

            if (GeometryHelper.IsRectanglesIntersects(ghostNewVertices, scene.Player.GetRotatedVertices()))
                return;
            Position = newPosition;
        }

        public static Ghost Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var animation = loader.GetAnimation("./animations/ghost");

            return new Ghost(position, size, directionAngle, animation);
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            var map = scene.Map;
            var player = scene.Player;
            var distance = Vector2.Distance(Position, player.Position);
            if (distance > 10)
                return;
            var hitWall = false;
            var angle = Math.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);
            var xRayUnit = (float) Math.Cos(angle);
            var yRayUnit = (float) Math.Sin(angle);
            var rayStep = 0.01f;
            var xStep = xRayUnit * rayStep;
            var yStep = yRayUnit * rayStep;
            var currentX = Position.X;
            var currentY = Position.Y;
            var rayLength = 0.0f;
            while (!hitWall && rayLength <= distance)
            {
                rayLength += rayStep;
                currentX += xStep;
                currentY += yStep;
                var testX = (int) currentX;
                var testY = (int) currentY;
                var cell = map.At(testY, testX);

                if (!CellTypes.walkable.Contains(cell.Type))
                    hitWall = true;
            }

            if (hitWall) return;
            DirectionAngle = (float) angle;
            Move(scene, elapsedMilliseconds);
        }
    }
}