using System;
using System.Linq;
using System.Runtime.CompilerServices;
using simple3d.Levels.Tools;
using simple3d.MathUtils;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class Scene25DRenderer : ISceneRenderer
    {
        private static readonly float[] depthBuffer = new float[2000];

        public void Render(IScreen screen, Scene scene, float elapsedMilliseconds, bool renderMap)
        {
            RenderWorld(screen, scene);
            RenderObjects(screen, scene, elapsedMilliseconds);
            if (renderMap)
                RenderMap(screen, scene);
        }

        private void RenderMap(IScreen screen, Scene scene)
        {
            var mapHeight = scene.Map.Height;
            var mapWidth = scene.Map.Width;
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var pixelPerBlock = Math.Min(screenHeight / mapHeight, screenWidth / mapWidth);

            (float y, float x) TranslateCords(float y, float x)
            {
                return (y * pixelPerBlock, x * pixelPerBlock);
            }

            void DrawPoint(float y, float x, bool f)
            {
                var (sy, sx) = TranslateCords(y, x);
                for (var dx = -1; dx < 2; dx++)
                {
                    for (var dy = -1; dy < 2; dy++)
                    {
                        if (!f)
                            screen.Draw((int) sy + dy, (int) sx + dx, 0, 255, 0);
                        else
                            screen.Draw((int) sy + dy, (int) sx + dx, 0x80, 0, 0x80);
                    }
                }
            }

            for (var i = 0; i < mapHeight; i++)
            {
                for (var j = 0; j < mapWidth; j++)
                {
                    var (sy, sx) = TranslateCords(i, j);
                    var cell = scene.Map.At(i, j);
                    if (cell.Type != MapCellType.Wall)
                        continue;

                    for (var k = 0; k < pixelPerBlock; k++)
                    {
                        for (var l = 0; l < pixelPerBlock; l++)
                        {
                            var ll = (int) sy + k;
                            var kk = (int) sx + l;
                            if (ll < 0 || ll >= screenHeight || kk < 0 || kk >= screenWidth)
                                continue;
                            screen.Draw(kk, ll, 255, 0, 0);
                        }
                    }
                }
            }

            foreach (var obj in scene.Objects.Concat(new[] {scene.Player}))
            {
                DrawPoint(obj.Position.X, obj.Position.Y, true);
                var rotatedVertices = obj.GetRotatedVertices();
                Console.WriteLine($"{obj} {obj.GetDistanceToPlayerSquared(scene.Player)}");
                foreach (var vertex in rotatedVertices)
                {
                    DrawPoint(vertex.X, vertex.Y, false);
                }
            }
        }

        private static void RenderWorld(IScreen screen, Scene scene)
        {
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var player = scene.Player;
            var directionAngle = player.DirectionAngle;
            var viewDistance = player.ViewDistance;

            for (var x = 0; x < screenWidth; x++)
            {
                var rayAngle = directionAngle - player.FieldOfView / 2 + ((float) x / screen.Width) * player.FieldOfView;
                var xRayUnit = MathF.Sin(rayAngle);
                var yRayUnit = MathF.Cos(rayAngle);
                var ray = ComputeRay(rayAngle, player, scene.Map, xRayUnit, yRayUnit);
                var cosinePerspectiveCorrection = MathF.Cos(directionAngle - ray.Angle);
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
        private static void RenderFloorAndCeilAt(IScreen screen, Scene scene, int screenHeight, int y, Player player,
            float cosinePerspectiveCorrection, float xRayUnit, float yRayUnit, int x)
        {
            var distance = screenHeight / (y - screenHeight / 2.0f);
            var currentX = player.Position.X + distance / cosinePerspectiveCorrection * xRayUnit;
            var currentY = player.Position.Y + distance / cosinePerspectiveCorrection * yRayUnit;
            var testX = (int) MathF.Floor(currentX);
            var testY = (int) MathF.Floor(currentY);
            var sampleX = currentX - testX;
            var sampleY = currentY - testY;
            // branch prediction will speed up this code
            // ReSharper disable once InvertIf
            if (scene.Map.InBound(testY, testX))
            {
                var cell = scene.Map.At(testY, testX);
                screen.Draw(y, x, cell.Sprite.GetSample(sampleY, sampleX));
                screen.Draw(screenHeight - y, x, cell.CeilingSprite.GetSample(sampleY, sampleX));
            }
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
            Player player,
            Map map,
            float xRayUnit,
            float yRayUnit)
        {
            var playerX = player.Position.X;
            var playerY = player.Position.Y;
            var viewDistance = player.ViewDistance;
            var rayLength = 0.0f;
            var hit = false;
            var sampleX = 0.0f;
            var testX = -1;
            var testY = -1;
            MapCell cell = default;
            var currentX = playerX;
            var currentY = playerY;
            var currentXStep = xRayUnit / 100.0f;
            var currentYStep = yRayUnit / 100.0f;

            while (!hit && rayLength < viewDistance)
            {
                rayLength += 0.01f;
                currentX += currentXStep;
                currentY += currentYStep;
                testX = (int) currentX;
                testY = (int) currentY;

                if (map.InBound(testY, testX))
                {
                    cell = map.At(testY, testX);

                    if (cell.Type != MapCellType.Wall)
                        continue;

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
                else
                {
                    rayLength = viewDistance;
                    break;
                }
            }

            return new Ray(rayLength, rayAngle, sampleX, testX, testY, cell);
        }

        private static void RenderObjects(IScreen screen, Scene scene, float elapsedMilliseconds)
        {
            var player = scene.Player;
            var directionAngle = player.DirectionAngle;
            var eyeX = MathF.Sin(directionAngle);
            var eyeY = MathF.Cos(directionAngle);
            const float pi2 = MathF.PI * 2;
            var playerAngle = MathF.Atan2(eyeY, eyeX);
            var fov = player.FieldOfView;
            var viewDistance = player.ViewDistance;
            var screenHeight = screen.Height;
            var screenWidth = screen.Width;
            var screenHeight2 = screenHeight / 2.0f;
            var halfFov = fov / 2;
            var fov15 = fov / 1.5f;

            foreach (var mapObject in scene.Objects.OrderByDescending(s => s.GetDistanceToPlayerSquared(player)))
            {
                var fromObjectToPlayer = mapObject.Position - player.Position;

                var angle = playerAngle - MathF.Atan2(fromObjectToPlayer.Y, fromObjectToPlayer.X);

                if (angle < -MathF.PI)
                {
                    angle += pi2;
                }

                if (angle > MathF.PI)
                {
                    angle -= pi2;
                }

                var inPlayerFov = MathF.Abs(angle) < fov15;

                if (!inPlayerFov)
                {
                    continue;
                }

                var cosinePerspectiveCorrection = MathF.Cos(angle);
                var distance = fromObjectToPlayer.Length() * cosinePerspectiveCorrection;
                if (distance > viewDistance)
                {
                    continue;
                }

                var ceil = (int) (screenHeight2 - screenHeight / distance);
                var floor = screenHeight - ceil;
                var height = floor - ceil;
                var sprite = mapObject.Sprite;
                var width = height / sprite.AspectRatio;
                var width2 = width / 2.0f;
                var middle = (0.5f * angle / halfFov + 0.5f) * screenWidth;
                var startX = (int) MathF.Max(0.0f, width2 - middle);
                var endX = (int) MathF.Min(screenWidth + width2 - middle - 0.5f, width - 0.5f);
                var sampleYStep = 1.0f / height - 1e-5f;
                var sampleXStep = 1.0f / width - 1e-5f;
                var sampleX = startX / width;
                var drawingCeil = ceil < 0 ? 0 : ceil;
                var startSampleX = (drawingCeil - ceil) / (float) height;
                var endY = (int) MathF.Min(screenHeight - ceil - 1, MathF.Min(height, screenHeight));

                for (var x = startX; x < endX; x++)
                {
                    sampleX += sampleXStep;
                    var column = (int) (middle + x - width2);

                    if (depthBuffer[column] < distance)
                        continue;

                    var sampleY = startSampleX;
                    for (var y = 0; y < endY; y++)
                    {
                        sampleY += sampleYStep;
                        var pixel = sprite.GetSample(sampleY, sampleX);
                        if ((pixel & 0xFF000000) == 0) //TODO: fix alpha channels at screen?
                        {
                            continue;
                        }
                        screen.Draw(drawingCeil + y, column, pixel);
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