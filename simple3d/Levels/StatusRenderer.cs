using System.Diagnostics;
using simple3d.Drawing;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class StatusRenderer : IStatusBarRenderer
    {
        private readonly Sprite barSprite;
        private readonly int statusBarHeight;

        public StatusRenderer(Sprite barSprite, int statusBarHeight)
        {
            this.barSprite = barSprite;
            this.statusBarHeight = statusBarHeight;
        }

        public void Dispose()
        {
            //nothing
        }

        public void Render(IScreen screen, Scene scene)
        {
            RenderWeapon(screen, scene);
            RenderStatusBar(screen, scene);
        }

        private void RenderWeapon(IScreen screen, Scene scene)
        {
            var weapon = scene.Player.CurrentWeapon;
            var sprite = weapon.Sprite;
            var startY = screen.Height - sprite.Height - statusBarHeight;
            var startX = screen.Width - sprite.Width;
            var spriteWidth = sprite.Width;
            var spriteHeight = sprite.Height;

            for (var y = 0; y < spriteHeight; y++)
            {
                for (var x = 0; x < spriteWidth; x++)
                {
                    var pixel = sprite.GetPixel(y, x);
                    if ((pixel & 0xFF000000) == 0)
                        continue;
                    screen.Draw(y + startY, x + startX, pixel);
                }
            }
        }

        private void RenderStatusBar(IScreen screen, Scene scene)
        {
            DrawSprite(screen);
            DrawStatusLines(screen, scene.Player, screen.Width / 4);
        }

        private void DrawStatusLines(IScreen screen, Player player, int linesWidth)
        {
            var linesHeight = statusBarHeight / 5;
            var boundSize = screen.Width / 20;

            var partSize = statusBarHeight / 7;

            DrawStatusLine(
                screen,
                player.Health / player.MaxHealth,
                screen.Height - statusBarHeight + partSize,
                boundSize,
                linesHeight, linesWidth,
                0xFF5349);

            DrawStatusLine(
                screen,
                player.SpellPoints / player.MaxSpellPoints,
                screen.Height - statusBarHeight + partSize * 3,
                boundSize,
                linesHeight, linesWidth,
                0x0000FF);


            DrawStatusLine(
                screen,
                player.Endurance / player.MaxEndurance,
                screen.Height - statusBarHeight + partSize * 5,
                boundSize,
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
            var startY = screen.Height - statusBarHeight;

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