using System.Diagnostics;
using simple3d.Drawing;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class StatusBarRenderer : IStatusBarRenderer
    {
        private readonly Sprite barSprite;
        private readonly int height;

        public StatusBarRenderer(Sprite barSprite, int height)
        {
            this.barSprite = barSprite;
            this.height = height;
        }

        public void Dispose()
        {
            //nothing
        }

        public void Render(IScreen screen, Scene scene)
        {
            DrawSprite(screen);
            DrawStatusLines(screen, scene.Player, screen.Width / 5);
        }

        private void DrawStatusLines(IScreen screen, Player player, int linesWidth)
        {
            var linesHeight = height / 6;
            var barMiddle = screen.Height - height / 2;
            var xStart = screen.Width / 20;

            DrawStatusLine(
                screen,
                player.Health / player.MaxHealth,
                barMiddle - linesHeight * 3 / 2,
                xStart,
                linesHeight, linesWidth,
                0xFF5349);

            DrawStatusLine(
                screen,
                player.Endurance / player.MaxEndurance,
                barMiddle + linesHeight * 3 / 2,
                xStart,
                linesHeight, linesWidth,
                0x006400);
        }

        private static void DrawStatusLine(
            IScreen screen, float percentile, int yStart, int xStart, int height, int width, int color)
        {
            var yEnd = yStart + height;
            var xEnd = xStart + width;
            var xBound = (int) (xStart + width * percentile);

            for (var y = yStart; y < yEnd; y++)
            {
                for (var x = xStart; x < xEnd; x++)
                {
                    if (x < xBound || y == yStart || y == yEnd - 1 || x == xStart || x == xEnd - 1)
                        screen.Draw(y, x, color);
                }
            }
        }

        private void DrawSprite(IScreen screen)
        {
            var screenHeight = screen.Height;
            var screenWidth = screen.Width;
            var startY = screen.Height - height;

            for (var y = startY; y < screenHeight; y++)
            {
                for (var x = 0; x < screenWidth; x++)
                {
                    screen.Draw(y, x, barSprite.GetPixel(y - startY, x));
                }
            }
        }
    }
}