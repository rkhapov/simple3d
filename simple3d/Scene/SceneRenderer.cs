using System;
using simple3d.Ui;

namespace simple3d.Scene
{
    public class SceneRenderer : ISceneRenderer
    {
        public void Render(IScreen screen, Level level, double elapsedMilliseconds)
        {
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var player = level.Player;
            var viewAngle = player.ViewAngle;

            for (var x = 0; x < screenWidth; x++)
            {
                var ray = ComputeRay(x, screen, player, level.Map);
                var ceil = (int) (screenHeight / 2.0 - screenHeight / (ray.Length * Math.Cos(viewAngle - ray.Angle)));
                var floor = screenHeight - ceil;

                for (var y = 0; y < screenHeight; y++)
                {
                    if (y <= ceil)
                    {
                        screen.Draw(y, x, 0, 0, 0x64);
                    }
                    else if (y > floor)
                    {
                        screen.Draw(y, x, 0, 0x64, 0);
                    }
                    else
                    {
                        screen.Draw(y, x, 0x64, 0, 0);
                    }
                }
            }
        }

        private struct Ray
        {
            public Ray(bool hit, double length, double angle)
            {
                Hit = hit;
                Length = length;
                Angle = angle;
            }

            public bool Hit { get; }
            public double Length { get; }
            public double Angle { get; }
        }

        private Ray ComputeRay(int x, IScreen screen, Player player, Map map)
        {
            var fov = player.FieldOfView;
            var playerX = player.X;
            var playerY = player.Y;
            var rayAngle = player.ViewAngle - fov / 2 + ((double) x / screen.Width) * fov;
            var xRayUnit = Math.Sin(rayAngle);
            var yRayUnit = Math.Cos(rayAngle);
            var viewDistance = player.ViewDistance;
            var rayLength = 0.0;
            var hit = false;

            while (!hit && rayLength < viewDistance)
            {
                rayLength += 0.01;
                var testX = (int) (playerX + xRayUnit * rayLength);
                var testY = (int) (playerY + yRayUnit * rayLength);

                if (!map.InBound(testY, testX))
                {
                    hit = true;
                    rayLength = viewDistance;
                    break;
                }

                if (map.At(testY, testX) == Cell.Wall)
                {
                    hit = true;
                }
            }

            return new Ray(hit, rayLength, rayAngle);
        }
    }
}