using System;
using System.Linq;
using simple3d.Drawing;
using simple3d.SDL2;
using simple3d.Tools;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class MonologueRenderer : IMonologueRenderer
    {
        private readonly ITextRenderer textRenderer;
        private readonly int statusBarHeight;
        private readonly int lineHeight;

        public MonologueRenderer(ITextRenderer textRenderer, int statusBarHeight)
        {
            this.textRenderer = textRenderer;
            this.statusBarHeight = statusBarHeight;
            lineHeight = textRenderer.RenderText("M", new SDL.SDL_Color()).Height + 5;
        }

        public void Render(IScreen screen, Scene scene)
        {
            if (scene.Player.CurrentMonologue == null || scene.Player.CurrentMonologue.IsOver)
            {
                return;
            }

            var lines = scene.Player.CurrentMonologue.CurrentText
                .Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => textRenderer.RenderText(l, new SDL.SDL_Color()))
                .ToArray();

            var startY = screen.Height - statusBarHeight - lines.Sum(k => lineHeight);

            foreach (var (line, i) in lines.Select((l, i) => (l, i)))
            {
                screen.DrawSprite(line, startY + lineHeight * i, screen.Width / 2 - line.Width / 2);
            }
        }
    }
}