using System;
using System.Linq;
using System.Numerics;
using objects.Collectables;
using objects.Monsters.Algorithms;
using objects.Weapons;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace objects.Monsters
{
    public abstract class BaseObject : IMapObject
    {
        protected BaseObject(Vector2 position, Vector2 size, float directionAngle)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
        }
        
        public Vector2 Position { get; protected set; }
        public Vector2 Size { get; protected set; }
        public float DirectionAngle { get; protected set; }
        public abstract Sprite Sprite { get; }

        public abstract void OnWorldUpdate(Scene scene, float elapsedMilliseconds);
        public abstract void OnLeftMeleeAttack(Scene scene, int damage);
        public abstract void OnRightMeleeAttack(Scene scene, int damage);
        public abstract void OnShoot(Scene scene, int damage);

        public virtual void DoLeftMeleeAttackOnPlayer(Scene scene, int damage)
        {
            scene.Player.OnLeftMeleeAttack(scene, damage);
        }

        public virtual void DoRightMeleeAttackOnPlayer(Scene scene, int damage)
        {
            scene.Player.OnRightMeleeAttack(scene, damage);
        }

        protected abstract int ViewDistance { get; }
        protected abstract float MoveSpeed { get; }

        protected bool CanSeePlayer(Scene scene)
        {
            var map = scene.Map;
            var player = scene.Player;
            var fromPlayerToUs = player.Position - Position;
            var lengthSquared = fromPlayerToUs.LengthSquared();

            if (lengthSquared > ViewDistance * ViewDistance)
                return false;

            var distance = MathF.Sqrt(lengthSquared);

            var hitWall = false;
            var angle = MathF.Atan2(fromPlayerToUs.Y, fromPlayerToUs.X);
            var xRayUnit = MathF.Cos(angle);
            var yRayUnit = MathF.Sin(angle);
            var rayStep = 0.01f;
            var xStep = xRayUnit * rayStep;
            var yStep = yRayUnit * rayStep;
            var currentX = Position.X;
            var currentY = Position.Y;
            var rayLength = 0.0f;

            while (!hitWall && rayLength < distance)
            {
                rayLength += rayStep;
                currentX += xStep;
                currentY += yStep;
                var testX = (int) currentX;
                var testY = (int) currentY;
                var cell = map.At(testY, testX);

                if (cell.Type != MapCellType.Empty)
                    hitWall = true;
            }

            return !hitWall;
        }

        [Flags]
        public enum MovingFlags
        {
            Standard = 1,
            IgnoreObjects = 2
        }

        public void MoveOnDirection(Scene scene, float elapsedTime, MovingFlags flags = MovingFlags.Standard)
        {
            var map = scene.Map;
            var dx = MathF.Cos(DirectionAngle) * MoveSpeed * elapsedTime;
            var dy = MathF.Sin(DirectionAngle) * MoveSpeed * elapsedTime;
            var newPosition = Position + new Vector2(dx, 0);

            if (CanMove(scene, newPosition, map, flags.HasFlag(MovingFlags.IgnoreObjects)))
                Position = newPosition;

            newPosition = Position + new Vector2(0, dy);

            if (CanMove(scene, newPosition, map, flags.HasFlag(MovingFlags.IgnoreObjects)))
                Position = newPosition;
        }

        private bool CanMove(Scene scene, Vector2 newPosition, Map map, bool ignoreObjects)
        {
            var newVertices = GeometryHelper.GetRotatedVertices(newPosition, Size, DirectionAngle);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var vertex in newVertices)
            {
                var testX = (int) vertex.X;
                var testY = (int) vertex.Y;

                if (!map.InBound(testY, testX) || map.At(testY, testX).Type == MapCellType.Wall || map.At(testY, testX).Type == MapCellType.Window)
                {
                    return false;
                }
            }

            if (!ignoreObjects)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var objectVertices in scene
                    .Objects
                    .Where(obj => obj != this && !(obj is BaseCollectable))
                    .Select(o => o.GetRotatedVertices()))
                {
                    if (GeometryHelper.IsRectanglesIntersects(newVertices, objectVertices))
                    {
                        return false;
                    }
                }
            }

            if (GeometryHelper.IsRectanglesIntersects(newVertices, scene.Player.GetRotatedVertices()))
                return false;

            return true;
        }
        
        protected float GetAngleToPoint(MapPoint point)
        {
            return MathF.Atan2(
                point.Y + 0.5f - Position.Y,
                point.X + 0.5f - Position.X);
        }
    }
}