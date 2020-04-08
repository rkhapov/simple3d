using System;
using System.Linq;
using simple3d.Ui;

namespace simple3d.Scene
{
    public class SceneRenderer : ISceneRenderer
    {
        private readonly Sprite wallSprite;
        private readonly Sprite skeletonSprite;
        private static readonly float[] depthBuffer = new float[1000];

        public SceneRenderer(Sprite wallSprite, Sprite skeletonSprite)
        {
            this.wallSprite = wallSprite;
            this.skeletonSprite = skeletonSprite;
        }

        public void Render(IScreen screen, Level level, float elapsedMilliseconds)
        {
            Render3dScene(screen, level);
            RenderSkeletons(screen, level.Player, level.Map);
        }

        private void Render3dScene(IScreen screen, Level level)
        {
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var player = level.Player;
            var viewAngle = player.ViewAngle;
            var viewDistance = player.ViewDistance;

            for (var x = 0; x < screenWidth; x++)
            {
                var ray = ComputeRay(x, screen, player, level.Map);
                var ceil = (int) (screenHeight / 2.0 - screenHeight / (ray.Length * MathF.Cos(viewAngle - ray.Angle)));
                var floor = screenHeight - ceil;
                depthBuffer[x] = ray.Length;

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
                            var sampleY = (y - ceil) / ((float) floor - ceil);
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
            public Ray(bool hit, float length, float angle, float sampleX)
            {
                Hit = hit;
                Length = length;
                Angle = angle;
                SampleX = sampleX;
            }

            public bool Hit { get; }
            public float Length { get; }
            public float Angle { get; }
            public float SampleX { get; }
        }

        private Ray ComputeRay(int x, IScreen screen, Player player, Map map)
        {
            var fov = player.FieldOfView;
            var playerX = player.X;
            var playerY = player.Y;
            var rayAngle = player.ViewAngle - fov / 2 + ((float) x / screen.Width) * fov;
            var xRayUnit = MathF.Sin(rayAngle);
            var yRayUnit = MathF.Cos(rayAngle);
            var viewDistance = player.ViewDistance;
            var rayLength = 0.0f;
            var hit = false;
            var sampleX = 0.0f;

            while (!hit && rayLength < viewDistance)
            {
                rayLength += 0.01f;
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
                    var blockMiddleX = testX + 0.5f;
                    var blockMiddleY = testY + 0.5f;
                    var angle = MathF.Atan2(currentY - blockMiddleY, currentX - blockMiddleX);
                    const float oneFourthOfPi = MathF.PI * 0.25f;
                    const float threeFourthOfPi = MathF.PI * 0.75f;

                    if (angle > -oneFourthOfPi && angle < oneFourthOfPi)
                    {
                        sampleX = currentY - testY;
                    }
                    else if (angle > oneFourthOfPi && angle < threeFourthOfPi)
                    {
                        sampleX = currentX - testX;
                    }
                    else if (angle < -oneFourthOfPi && angle > -threeFourthOfPi)
                    {
                        sampleX = currentX - testX;
                    }
                    else
                    {
                        sampleX = currentY - testY;
                    }
                }
            }

            return new Ray(hit, rayLength, rayAngle, sampleX);
        }

        private void RenderSkeletons(IScreen screen, Player player, Map map)
        {
            var playerX = player.X;
            var playerY = player.Y;
            var eyeX = MathF.Sin(player.ViewAngle);
            var eyeY = MathF.Cos(player.ViewAngle);
            var pi2 = MathF.PI * 2;
            var playerAngle = MathF.Atan2(eyeY, eyeX);
            var fov = player.FieldOfView;
            var viewDistance = player.ViewDistance;
            var screenHeight = screen.Height;
            var screenWidth = screen.Width;
            var aspectRatio = (float) skeletonSprite.Height / skeletonSprite.Width;
            var halfFov = fov / 2;

            foreach (var skeleton in map.GetSkeletons().OrderByDescending(s =>
            {
                var dx = s.X - playerX;
                var dy = s.Y - playerY;
                return MathF.Sqrt(dx * dx + dy * dy);
            }))
            {
                var dx = skeleton.X - playerX;
                var dy = skeleton.Y - playerY;
                var distance = MathF.Sqrt(dx * dx + dy * dy);

                if (distance > viewDistance || distance < 0.5f)
                {
                    continue;
                }

                var angle = playerAngle - MathF.Atan2(dy, dx);

                if (angle < -MathF.PI)
                {
                    angle += pi2;
                }

                if (angle > MathF.PI)
                {
                    angle -= pi2;
                }

                var inPlayerFov = MathF.Abs(angle) < halfFov;

                if (!inPlayerFov)
                    continue;
                
                var ceil = (int) (screenHeight / 2.0f - screenHeight / distance);
                var floor = screenHeight - ceil;
                var height = floor - ceil;
                if (ceil < 0 || floor < 0 || height < 0)
                    continue;
                var width = height / aspectRatio;
                var middle = (0.5f * angle / halfFov + 0.5f) * screenWidth;

                for (var x = 0; x < width; x++)
                {
                    var sampleX = x / width;
                    var column = (int) (middle + x - width / 2.0f);
                    if (column < 0 || column >= screenWidth || depthBuffer[column] < distance)
                        continue;

                    for (var y = 0; y < height; y++)
                    {
                        if (ceil + y >= screenHeight)
                            continue;
                        var sampleY = y / (float) height;
                        var pixel = skeletonSprite.GetSample(sampleY, sampleX);
                        if ((pixel & 0xFF000000) == 0)
                        {
                            continue;
                        }
                        screen.Draw(ceil + y, column, pixel);
                    }
                }
            }
        }

        public void Dispose()
        {
            wallSprite?.Dispose();
        }
    }
}