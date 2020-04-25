using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using simple3d.Drawing;
using simple3d.SDL2;
using simple3d.Tools;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class StatusRenderer : IStatusBarRenderer
    {
        private readonly Sprite barSprite;
        private readonly Sprite crossSprite;
        private readonly ITextRenderer logTextRenderer;
        private readonly int statusBarHeight;

        public StatusRenderer(
            Sprite barSprite,
            Sprite crossSprite,
            int statusBarHeight,
            ITextRenderer logTextRenderer)
        {
            this.barSprite = barSprite;
            this.crossSprite = crossSprite;
            this.statusBarHeight = statusBarHeight;
            this.logTextRenderer = logTextRenderer;
        }

        public void Dispose()
        {
            //nothing
        }

        public void Render(IScreen screen, Scene scene)
        {
            var task1 = Task.Run(() => RenderWeapon(screen, scene));
            var task2 = Task.Run(() => RenderStatusBar(screen, scene));
            var task3 = Task.Run(() => RenderCross(screen));
            var task4 = Task.Run(() => RenderLog(screen, scene));

            Task.WhenAll(task1, task2, task3, task4).Wait();
        }

        private void RenderLog(IScreen screen, Scene scene)
        {
            var currentY = 0;

            foreach (var msg in scene.EventsLogger.GetMessages())
            {
                var textSprite = logTextRenderer.RenderText(msg,
                    new SDL.SDL_Color {a = 0, b = 255, g = 255, r = 255});

                screen.DrawSprite(textSprite, currentY, 0);

                currentY += textSprite.Height;
            }
        }

        private void RenderCross(IScreen screen)
        {
            if (crossSprite == null)
                return;

            screen.DrawSprite(
                crossSprite,
                screen.Height / 2 - crossSprite.Height / 2,
                screen.Width / 2 - crossSprite.Width / 2);
        }

        private void RenderWeapon(IScreen screen, Scene scene)
        {
            var weapon = scene.Player.Weapon;
            if (weapon == null) return;
            var sprite = weapon.Sprite;
            var height = screen.Height / 2;
            var width = (int) (height / sprite.AspectRatio);
            var startY = screen.Height - height - statusBarHeight;
            var startX = screen.Width / 2 + width / 2;
            if (startX + width > screen.Width)
                startX = screen.Width / 2;
            var yStep = 1.0f / height;
            var xStep = 1.0f / width;
            var currentY = 0.0f;

            for (var y = 0; y < height; y++)
            {
                var currentX = 0.0f;
                for (var x = 0; x < width; x++)
                {
                    var pixel = sprite.GetSample(currentY, currentX);
                    currentX += xStep;
                    if ((pixel & 0xFF000000) == 0)
                        continue;
                    screen.Draw(y + startY, x + startX, pixel);
                }

                currentY += yStep;
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
            screen.DrawSprite(barSprite, screen.Height - statusBarHeight, 0);
        }
    }
}