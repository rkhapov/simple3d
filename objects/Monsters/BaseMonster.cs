﻿using System;
using System.Linq;
using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace objects.Monsters
{
    public abstract class BaseMonster : IMapObject
    {
        public BaseMonster(Vector2 position, Vector2 size, float directionAngle, int health)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
            Health = health;
        }

        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public void ReceiveDamage(int damage)
        {
            Health -= damage;
        }

        public Vector2 Position { get; protected set; }
        public Vector2 Size { get; }
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

        protected bool HaveWallOnStraightWayToPlayer(Scene scene)
        {
            var map = scene.Map;
            var player = scene.Player;
            var fromUsToPlayer = Position - player.Position;
            var lengthSquared = fromUsToPlayer.LengthSquared();

            if (lengthSquared > ViewDistance * ViewDistance)
                return true;

            var distance = MathF.Sqrt(lengthSquared);

            var hitWall = false;
            var angle = MathF.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);
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

                if (cell.Type == MapCellType.Wall)
                    hitWall = true;
            }

            return hitWall;
        }

        protected void MoveOnDirection(Scene scene, float elapsedTime)
        {
            var map = scene.Map;
            var dx = MathF.Cos(DirectionAngle) * MoveSpeed * elapsedTime;
            var dy = MathF.Sin(DirectionAngle) * MoveSpeed * elapsedTime;
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

                if (!map.InBound(testY, testX) || map.At(testY, testX).Type == MapCellType.Wall || map.At(testY, testX).Type == MapCellType.Window)
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
    }
}