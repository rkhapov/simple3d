using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using simple3d.Levels.Tools;
using simple3d.Tools;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class Scene25DRenderer : ISceneRenderer
    {
        private static readonly float[] ZBuffer = new float[4000];

        public void Render(IScreen screen, Scene scene, float elapsedMilliseconds)
        {
            scene.Map.UpdateMap(elapsedMilliseconds);
            RenderWorld(screen, scene, elapsedMilliseconds);
            RenderObjectsOneThread(screen, scene, elapsedMilliseconds);
        }

        private static void RenderWorld(IScreen screen, Scene scene, float elapsedMilliseconds)
        {
            
            RenderWorldParallel(screen, scene);
        }

        private static void RenderWorldParallel(IScreen screen, Scene scene)
        {
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var player = scene.Player;
            var directionAngle = player.DirectionAngle;
            var viewDistance = player.ViewDistance;

            Task.WhenAll(Enumerable
                .Range(0, screenWidth)
                .Select(x => Task.Run(() =>
                {
                    var rayAngle = directionAngle - player.FieldOfView / 2 +
                                   ((float) x / screen.Width) * player.FieldOfView;
                    var xRayUnit = MathF.Sin(rayAngle);
                    var yRayUnit = MathF.Cos(rayAngle);
                    var ray = ComputeRay(rayAngle, player, scene.Map, xRayUnit, yRayUnit);
                    var cosinePerspectiveCorrection = MathF.Cos(directionAngle - ray.Angle);
                    ZBuffer[x] = ray.Targets.Peek().Length;
              
                    while (ray.Targets.Count != 0)
                    {
                        var target = ray.Targets.Pop();
                        var ceil =
                            (int)(screenHeight / 2.0 - screenHeight / (target.Length * cosinePerspectiveCorrection));
                        var floor = screenHeight - ceil;
                        var wallHeight = (float)floor - ceil;
                        var wallCeil = ceil < 0 ? 0 : ceil + 1;
                        var wallFloor = floor > screenHeight ? screenHeight : floor;
                        
                        for (var y = wallFloor; y < screenHeight; y++)
                        {
                            RenderFloorAndCeilAt(screen, scene, screenHeight, y, player, cosinePerspectiveCorrection,
                                xRayUnit, yRayUnit, x);
                        }

                        for (var y = wallCeil; y < wallFloor; y++)
                        {
                            RenderWallAt(screen, target, viewDistance, y, ceil, wallHeight, x);
                        }
                    }
                }))).Wait();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RenderWallAt(IScreen screen, Ray.Target ray, float viewDistance, int y, int ceil, float wallHeight, int x)
        {
            if (ray.Length < viewDistance)
            {
                var sampleY = (y - ceil) / wallHeight;
                var pixel = ray.MapCell.WallSprite.GetSample(sampleY, ray.SampleX);
                if (pixel.IsTransparentColor()) //TODO: fix alpha channels at screen?
                {
                    return;
                }
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
                screen.Draw(y, x, cell.FloorSprite.GetSample(sampleY, sampleX));
                screen.Draw(screenHeight - y, x, cell.CeilingSprite.GetSample(sampleY, sampleX));
            }
        }

        private struct Ray
        {
            public Ray(float angle)
            {
                Angle = angle;
                Targets = new Stack<Target>();
            }


            public float Angle { get; }

            public Stack<Target> Targets { get; }



            public struct Target
            {
                public Target(float length, float sampleX, int wallX, int wallY, MapCell mapCell)
                {
                    Length = length;
                    SampleX = sampleX;
                    WallX = wallX;
                    WallY = wallY;
                    MapCell = mapCell;
                }
                public float Length { get; }
                public float SampleX { get; }
                public int WallX { get; }
                public int WallY { get; }
                public MapCell MapCell { get; }
            }

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
            var rayStep = 0.01f;
            var currentX = playerX;
            var currentY = playerY;
            var currentXStep = xRayUnit * rayStep;
            var currentYStep = yRayUnit * rayStep;
            var ray = new Ray(rayAngle);

            while (!hit && rayLength < viewDistance)
            {
                rayLength += rayStep;
                currentX += currentXStep;
                currentY += currentYStep;
                testX = (int) currentX;
                testY = (int) currentY;

                if (map.InBound(testY, testX))
                {
                    cell = map.At(testY, testX);

                    if (cell.Type == MapCellType.Empty)//mb
                        continue;

                    if (!CellTypes.transparents.Contains(cell.Type))
                        hit = true;

             
                    if (CellTypes.transparents.Contains(cell.Type ) && ray.Targets.Count != 0 && ray.Targets.Peek().MapCell.Type == cell.Type)
                        continue;

                   
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
                        sampleX = 1 - (currentX - testX);
                    }
                    else if (angle < -oneFourthOfPi && angle > -threeFourthOfPi)
                    {
                        sampleX = currentX - testX;
                    }
                    else
                    {
                        sampleX = 1 - (currentY - testY);
                    }
                    ray.Targets.Push(new Ray.Target(rayLength, sampleX, testX, testY, cell));
                }
                else
                {
                    rayLength = viewDistance;
                    break;
                }
            }

            return ray;
        }

        private static void RenderObjectsOneThread(IScreen screen, Scene scene, float elapsedMilliseconds)
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

                    if (column < 0 || ZBuffer[column] < distance)
                        continue;

                    var sampleY = startSampleX;
                    for (var y = 0; y < endY; y++)
                    {
                        sampleY += sampleYStep;
                        var pixel = sprite.GetSample(sampleY, sampleX);
                        if (pixel.IsTransparentColor())
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