using System;

namespace simple3d.Levels
{
    public class PlayerCamera
    {
        public float X = 1.0f;
        public float Y = 1.0f;
        public float ViewAngle = 0.0f;
        public readonly float FieldOfView = MathF.PI / 3;
        public readonly float ViewDistance = 30.0f;
        public readonly float MovingSpeed = 0.005f;
    }
}