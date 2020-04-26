using System.Diagnostics;
using System.Threading.Tasks;
using simple3d.Drawing;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class StatusRenderer : IStatusBarRenderer
    {
        private readonly Sprite barSprite;
        private readonly Sprite bowSprite;
        private readonly Sprite swordSprite;
        private readonly Sprite crossSprite;
        private readonly Sprite frameSprite;
        private readonly int statusBarHeight;

        public StatusRenderer(Sprite barSprite, Sprite crossSprite, int statusBarHeight,
            Sprite bowSprite, Sprite swordSprite, Sprite frameSprite)
        {
            this.barSprite = barSprite;
            this.bowSprite = bowSprite;
            this.swordSprite = swordSprite;
            this.crossSprite = crossSprite;
            this.statusBarHeight = statusBarHeight;
            this.frameSprite = frameSprite;
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
            
            Task.WhenAll(task1, task2).Wait();
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
            DrawStatusLines(screen, scene.Player, screen.Width / 8);
            DrawWeaponMiniature(screen, scene);
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
                0xFF2020,
                0xFF3C3C
                );

            DrawStatusLine(
                screen,
                player.SpellPoints / player.MaxSpellPoints,
                screen.Height - statusBarHeight + partSize * 3,
                boundSize,
                linesHeight, linesWidth,
                0x0000FF,
                0x00096);


            DrawStatusLine(
                screen,
                player.Endurance / player.MaxEndurance,
                screen.Height - statusBarHeight + partSize * 5,
                boundSize,
                linesHeight, linesWidth,
                0x009600,
                0x006400
            );
        }

        private static void DrawStatusLine(
            IScreen screen, float percentile, int yStart, int xStart, int height, int width, int color, int color2)
        {
            var yEnd = yStart + height;
            var xEnd = xStart + width;
            var xBound = (int) (xStart + width * percentile);

            for (var y = yStart; y < yEnd; y++)
            {
                for (var x = xStart; x < xEnd; x++)
                {
                    if (x < xBound - 1 || y <= yStart + 1 || y >= yEnd - 2 || x <= xStart + 1 || x >= xEnd - 2)
                        screen.Draw(y, x, color);
                    else
                    {
                        screen.Draw(y, x, color2);
                    }
                }
            }
        }

        private void DrawSprite(IScreen screen)
        {
            screen.DrawSprite(barSprite, screen.Height - statusBarHeight, 0);
        }

        private void DrawFrame(IScreen screen, int y, int x)
        {
            screen.DrawSprite(frameSprite, y, x);
        }

        private void DrawWeaponMiniature(IScreen screen, Scene scene)
        {
            var weaponSpriteY = screen.Height - statusBarHeight;
            var weaponSpriteX = 700;
            DrawFrame(screen,screen.Height - statusBarHeight , 700);
            if (scene.Player.Weapon is MeleeWeapon)
                screen.DrawSprite(swordSprite, weaponSpriteY, weaponSpriteX);
            else 
                screen.DrawSprite(bowSprite, weaponSpriteY, weaponSpriteX);
        }
    }
}