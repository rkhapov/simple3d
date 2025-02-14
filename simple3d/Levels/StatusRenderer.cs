﻿using System.Diagnostics;
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
        private readonly Sprite bowSprite;
        private readonly Sprite swordSprite;
        private readonly Sprite crossSprite;
        private readonly Sprite frameSprite;
        private readonly ITextRenderer logTextRenderer;
        private readonly int statusBarHeight;
        private readonly INotesRenderer notesRenderer;
        private readonly IMonologueRenderer monologueRenderer;
        private readonly ITextRenderer statusTextRenderer;
        private readonly Sprite arrowSprite;
        private readonly Sprite fireBallSprite;
        private readonly Sprite shockBallSprite;
        private readonly Sprite faceSprite;
        private readonly Sprite faceHurtedSprite;
        private readonly Sprite faceBadSprite;

        public StatusRenderer(
            Sprite barSprite,
            Sprite crossSprite,
            int statusBarHeight,
            ITextRenderer logTextRenderer,
            INotesRenderer notesRenderer,
            IMonologueRenderer monologueRenderer,
            Sprite bowSprite,
            Sprite swordSprite,
            Sprite frameSprite,
            ITextRenderer textRenderer,
            Sprite arrowSprite,
            Sprite fireBallSprite,
            Sprite shockBallSprite,
            Sprite faceSprite,
            Sprite faceHurtedSprite,
            Sprite faceBadSprite)
        {
            this.barSprite = barSprite;
            this.bowSprite = bowSprite;
            this.swordSprite = swordSprite;
            this.crossSprite = crossSprite;
            this.statusBarHeight = statusBarHeight;
            this.logTextRenderer = logTextRenderer;
            this.frameSprite = frameSprite;
            this.notesRenderer = notesRenderer;
            this.monologueRenderer = monologueRenderer;
            this.statusTextRenderer = textRenderer;
            this.arrowSprite = arrowSprite;
            this.fireBallSprite = fireBallSprite;
            this.shockBallSprite = shockBallSprite;
            this.faceSprite = faceSprite;
            this.faceHurtedSprite = faceHurtedSprite;
            this.faceBadSprite = faceBadSprite;
        }

        public void Dispose()
        {
            //nothing
        }

        public void Render(IScreen screen, Scene scene)
        {
            var task1 = Task
                .Run(() => RenderWeapon(screen, scene))
                .ContinueWith((_) => monologueRenderer?.Render(screen, scene));
            var task2 = Task.Run(() => RenderStatusBar(screen, scene));
            var task3 = Task.Run(() => RenderCross(screen));
            var task4 = Task.Run(() => RenderLog(screen, scene));
            var task5 = Task.Run(() => notesRenderer?.Render(screen, scene));

            Task.WhenAll(task1, task2, task3, task4, task5).Wait();
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
            DrawStatusLines(screen, scene.Player, screen.Width / 8);
            DrawWeaponMiniature(screen, scene);
            DrawSpellMiniature(screen, scene);
            DrawFaceMiniature(screen, scene);
            DrawArrowsCount(screen, scene, statusTextRenderer);
            DrawArrows(screen);
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
                0xF5001D,
                0x9F0013
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
            DrawFrame(screen,weaponSpriteY , weaponSpriteX);
            if (scene.Player.Weapon is MeleeWeapon)
                screen.DrawSprite(swordSprite, weaponSpriteY, weaponSpriteX);
            else 
                screen.DrawSprite(bowSprite, weaponSpriteY, weaponSpriteX);
        }

        private void DrawSpellMiniature(IScreen screen, Scene scene)
        {
            var spellSpriteY = screen.Height - statusBarHeight;
            var spellSpriteX = 550;
            DrawFrame(screen, spellSpriteY, spellSpriteX);
            if (scene.Player.CurrentSpell == Spells.FireBall)
                screen.DrawSprite(fireBallSprite, spellSpriteY, spellSpriteX);
            else
                screen.DrawSprite(shockBallSprite, spellSpriteY, spellSpriteX);
        }

        private void DrawFaceMiniature(IScreen screen, Scene scene)
        {
            var faceSpriteY = screen.Height - statusBarHeight;
            var faceSpriteX = 400;
            DrawFrame(screen, faceSpriteY, faceSpriteX);
            if (scene.Player.Health <= 32 && scene.Player.Health >= 23)
                screen.DrawSprite(faceSprite, faceSpriteY, faceSpriteX + 10);
            else if (scene.Player.Health < 23 && scene.Player.Health >= 12)
                screen.DrawSprite(faceHurtedSprite, faceSpriteY, faceSpriteX + 10);
            else if (scene.Player.Health < 12)
                screen.DrawSprite(faceBadSprite, faceSpriteY, faceSpriteX + 10);
        }

        private void DrawArrowsCount(IScreen screen, Scene scene, ITextRenderer textRenderer)
        {
            var arrowsCountY = screen.Height - statusBarHeight + 30;
            var arrowsCountX = 1000;
            var currentAmountOfArrows = scene.Player.CurrentAmountOfArrows;
            var maxAmountOfArrows = scene.Player.MaxAmountOfArrows;
            textRenderer.RenderText($"{currentAmountOfArrows} / {maxAmountOfArrows}", new SDL.SDL_Color(), screen, arrowsCountY,
                arrowsCountX);
        }

        private void DrawArrows(IScreen screen)
        {
            var y = screen.Height - statusBarHeight;
            var arrow1Y = y + 10;
            var arrow1X = 850;
            var arrow2Y = y + 30;
            var arrow3Y = y + 50;
            screen.DrawSprite(arrowSprite, arrow1Y, arrow1X);
            screen.DrawSprite(arrowSprite, arrow2Y, arrow1X);
            screen.DrawSprite(arrowSprite, arrow3Y, arrow1X);
        }
    }
}