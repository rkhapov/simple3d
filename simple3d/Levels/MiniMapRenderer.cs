using System;
using System.Linq;
using simple3d.MathUtils;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class MiniMapRenderer : IMiniMapRenderer
    {
        public void Dispose()
        {
            //nothing
        }

        public void Render(IScreen screen, Scene scene)
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
                foreach (var vertex in rotatedVertices)
                {
                    DrawPoint(vertex.X, vertex.Y, false);
                }
            }
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
                foreach (var vertex in rotatedVertices)
                {
                    DrawPoint(vertex.X, vertex.Y, false);
                }
            }
        }
    }
}