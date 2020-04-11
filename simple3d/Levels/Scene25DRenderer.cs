using System;
using System.Linq;
using System.Runtime.CompilerServices;
using simple3d.Levels.Tools;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class Scene25DRenderer : ISceneRenderer
    {
        private static readonly float[] depthBuffer = new float[2000];

        public void Render(IScreen screen, Scene scene, float elapsedMilliseconds)
        {
            RenderWorld(screen, scene);
            RenderObjects(screen, scene, elapsedMilliseconds);
        }

        private void RenderWorld(IScreen screen, Scene scene)
        {
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var player = scene.PlayerCamera;
            var viewAngle = player.ViewAngle;
            var viewDistance = player.ViewDistance;

            for (var x = 0; x < screenWidth; x++)
            {
                var rayAngle = player.ViewAngle - player.FieldOfView / 2 + ((float) x / screen.Width) * player.FieldOfView;
                var xRayUnit = MathF.Sin(rayAngle);
                var yRayUnit = MathF.Cos(rayAngle);
                var ray = ComputeRay(rayAngle, player, scene.Map, xRayUnit, yRayUnit);
                var cosinePerspectiveCorrection = MathF.Cos(viewAngle - ray.Angle);
                var ceil = (int) (screenHeight / 2.0 - screenHeight / (ray.Length * cosinePerspectiveCorrection));
                var floor = screenHeight - ceil;
                var wallHeight = (float) floor - ceil;
                var wallCeil = ceil < 0 ? 0 : ceil + 1;
                var wallFloor = floor > screenHeight ? screenHeight : floor;

                depthBuffer[x] = ray.Length;

                for (var y = wallFloor; y < screenHeight; y++)
                {
                    RenderFloorAndCeilAt(screen, scene, screenHeight, y, player, cosinePerspectiveCorrection, xRayUnit, yRayUnit, x);
                }

                for (var y = wallCeil; y < wallFloor; y++)
                {
                    RenderWallAt(screen, ray, viewDistance, y, ceil, wallHeight, x);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RenderWallAt(IScreen screen, Ray ray, float viewDistance, int y, int ceil, float wallHeight, int x)
        {
            if (ray.Length < viewDistance)
            {
                var sampleY = (y - ceil) / wallHeight;
                var pixel = ray.MapCell.Sprite.GetSample(sampleY, ray.SampleX);
                screen.Draw(y, x, pixel);
            }
            else
            {
                screen.Draw(y, x, 0, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RenderFloorAndCeilAt(IScreen screen, Scene scene, int screenHeight, int y, PlayerCamera player,
            float cosinePerspectiveCorrection, float xRayUnit, float yRayUnit, int x)
        {
            var distance = screenHeight / (y - screenHeight / 2.0f);
            var currentX = player.X + distance / cosinePerspectiveCorrection * xRayUnit;
            var currentY = player.Y + distance / cosinePerspectiveCorrection * yRayUnit;
            var testX = (int) Math.Floor(currentX);
            var testY = (int) Math.Floor(currentY);
            var sampleX = currentX - testX;
            var sampleY = currentY - testY;
            if (!scene.Map.InBound(testY, testX))
                return;
            var cell = scene.Map.At(testY, testX);
            screen.Draw(y, x, cell.Sprite.GetSample(sampleY, sampleX));
            screen.Draw(screenHeight - y, x, cell.CeilingSprite.GetSample(sampleY, sampleX));
        }

        private struct Ray
        {
            public Ray(float length, float angle, float sampleX, int wallX, int wallY, MapCell mapCell)
            {
                Length = length;
                Angle = angle;
                SampleX = sampleX;
                WallX = wallX;
                WallY = wallY;
                MapCell = mapCell;
            }

            public float Length { get; }
            public float Angle { get; }
            public float SampleX { get; }
            public int WallX { get; }
            public int WallY { get; }
            public MapCell MapCell { get; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Ray ComputeRay(
            float rayAngle,
            PlayerCamera playerCamera,
            Map map,
            float xRayUnit,
            float yRayUnit)
        {
            var playerX = playerCamera.X;
            var playerY = playerCamera.Y;
            var viewDistance = playerCamera.ViewDistance;
            var rayLength = 0.0f;
            var hit = false;
            var sampleX = 0.0f;
            var testX = -1;
            var testY = -1;
            MapCell cell = default;

            while (!hit && rayLength < viewDistance)
            {
                rayLength += 0.01f;
                var currentX = playerX + xRayUnit * rayLength;
                var currentY = playerY + yRayUnit * rayLength;
                testX = (int) currentX;
                testY = (int) currentY;

                if (!map.InBound(testY, testX))
                {
                    rayLength = viewDistance;
                    break;
                }

                cell = map.At(testY, testX);

                if (cell.Type == MapCellType.Wall)
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

            return new Ray(rayLength, rayAngle, sampleX, testX, testY, cell);
        }

        private static void RenderObjects(IScreen screen, Scene scene, float elapsedMilliseconds)
        {
            var player = scene.PlayerCamera;
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
            var halfFov = fov / 2;

            foreach (var mapObject in scene.Objects.OrderByDescending(s => s.GetDistanceToPlayer(player)))
            {
                var dx = mapObject.PositionX - playerX;
                var dy = mapObject.PositionY - playerY;
                var distance = MathF.Sqrt(dx * dx + dy * dy);

                if (distance > viewDistance)
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
                var sprite = mapObject.Sprite;
                var width = height / sprite.AspectRatio;
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
                        var pixel = sprite.GetSample(sampleY, sampleX);
                        if ((pixel & 0xFF000000) == 0) //TODO: fix alpha channels at screen?
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
            //empty
        }
    }
}