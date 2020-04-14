using System;
using System.Numerics;
using simple3d.Drawing;

namespace simple3d.Levels
{
    public abstract class Player : IMapObject
    {
        public readonly float FieldOfView = MathF.PI / 3;
        public readonly float ViewDistance = 30.0f;
        public readonly float MovingSpeed = 0.005f;

        protected Player(Vector2 position, Vector2 size, float directionAngle)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
        }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public float DirectionAngle { get; set; }
        public Sprite Sprite => throw new InvalidOperationException("Player should not be drawn");

        public abstract void OnWorldUpdate(Scene scene, float elapsedMilliseconds);
    }
}