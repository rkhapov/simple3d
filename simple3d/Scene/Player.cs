using System;

namespace simple3d.Scene
{
    public class Player
    {
        public double X = 1.0;
        public double Y = 1.0;
        public double ViewAngle = 0.0;
        public readonly double FieldOfView = MathF.PI / 3;
        public readonly double ViewDistance = 30.0;
        public readonly double MovingSpeed = 0.005;
    }
}