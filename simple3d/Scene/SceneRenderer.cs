using System;
using simple3d.Ui;

namespace simple3d.Scene
{
    public class SceneRenderer : ISceneRenderer
    {
        private readonly Sprite wallSprite;

        public SceneRenderer(Sprite wallSprite)
        {
            this.wallSprite = wallSprite;
        }

        public void Render(IScreen screen, Level level, double elapsedMilliseconds)
        {
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var player = level.Player;
            var viewAngle = player.ViewAngle;
            var viewDistance = player.ViewDistance;

            for (var x = 0; x < screenWidth; x++)
            {
                var ray = ComputeRay(x, screen, player, level.Map);
                var ceil = (int) (screenHeight / 2.0 - screenHeight / (ray.Length * Math.Cos(viewAngle - ray.Angle)));
                var floor = screenHeight - ceil;

                for (var y = 0; y < screenHeight; y++)
                {
                    if (y <= ceil)
                    {
                        screen.Draw(y, x, 0, 0, 0);
                    }
                    else if (y > floor)
                    {
                        screen.Draw(y, x, 0, 0x64, 0);
                    }
                    else
                    {
                        if (ray.Length < viewDistance)
                        {
                            var sampleY = (y - ceil) / ((double) floor - ceil);
                            var wallPixel = wallSprite.GetSample(sampleY, ray.SampleX);
                            screen.Draw(y, x, wallPixel);
                        }
                        else
                        {
                            screen.Draw(y, x, 0, 0, 0);
                        }
                    }
                }
            }
        }

        private struct Ray
        {
            public Ray(bool hit, double length, double angle, double sampleX)
            {
                Hit = hit;
                Length = length;
                Angle = angle;
                SampleX = sampleX;
            }

            public bool Hit { get; }
            public double Length { get; }
            public double Angle { get; }
            public double SampleX { get; }
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
            var sampleX = 0.0;

            while (!hit && rayLength < viewDistance)
            {
                rayLength += 0.01;
                var currentX = playerX + xRayUnit * rayLength;
                var currentY = playerY + yRayUnit * rayLength;
                var testX = (int) currentX;
                var testY = (int) currentY;

                if (!map.InBound(testY, testX))
                {
                    hit = true;
                    rayLength = viewDistance;
                    break;
                }

                if (map.At(testY, testX) == Cell.Wall)
                {
                    hit = true;
                    var blockMiddleX = testX + 0.5;
                    var blockMiddleY = testY + 0.5;
                    var angle = Math.Atan2(currentY - blockMiddleY, currentX - blockMiddleX);
                    const double oneFourthOfPi = Math.PI * 0.25;
                    const double threeFourthOfPi = Math.PI * 0.75;

                    if (angle > -oneFourthOfPi && angle < oneFourthOfPi) {
                        sampleX = currentY - testY;
                    }
                    else if (angle > oneFourthOfPi && angle < threeFourthOfPi) {
                        sampleX = currentX - testX;
                    }
                    else if (angle < -oneFourthOfPi && angle > -threeFourthOfPi) {
                        sampleX = currentX - testX;
                    }
                    else {
                        sampleX = currentY - testY;
                    }
                }
            }

            return new Ray(hit, rayLength, rayAngle, sampleX);
        }

        public void Dispose()
        {
            wallSprite?.Dispose();
        }
    }
}