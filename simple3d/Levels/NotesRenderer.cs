using System;
using System.Linq;
using System.Threading.Tasks;
using simple3d.Drawing;
using simple3d.SDL2;
using simple3d.Tools;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class NotesRenderer : INotesRenderer
    {
        private readonly Sprite noteSprite;
        private readonly ITextRenderer textRenderer;
        private readonly int statusBarHeight;
        private readonly int lineHeight;

        public NotesRenderer(Sprite noteSprite, ITextRenderer textRenderer, int statusBarHeight)
        {
            this.noteSprite = noteSprite;
            this.textRenderer = textRenderer;
            this.statusBarHeight = statusBarHeight;
            lineHeight = textRenderer.RenderText("M", new SDL.SDL_Color()).Height + 5;
        }

        public void Render(IScreen screen, Scene scene)
        {
            if (scene.Player.State != Player.PlayerState.TextReading)
            {
                return;
            }

            var visiblePartHeight = screen.Height - statusBarHeight;
            var height = visiblePartHeight;
            var width = (int) (height / noteSprite.AspectRatio);
            var startY = (visiblePartHeight - height) / 2;
            var startX = (screen.Width - width) / 2;
            var sampleYStep = 1.0f / height;
            var sampleXStep = 1.0f / width;
            var endY = startY + height;
            var endX = startX + width;
            var sampleY = 0.0f;

            for (var y = startY; y < endY; y++)
            {
                var sampleX = 0.0f;
                for (var x = startX; x < endX; x++)
                {
                    var pixel = noteSprite.GetSample(sampleY, sampleX);
                    if (!pixel.IsTransparentColor())
                        screen.Draw(y, x, pixel);
                    sampleX += sampleXStep;
                }

                sampleY += sampleYStep;
            }

            var noteStartY = (int) (startY + screen.Height / 10);
            var noteStartX = (int) (startX * 1.2f);
            var currentY = noteStartY;
            var black = new SDL.SDL_Color() {a = 0, b = 0, g = 0, r = 0};

            foreach (var line in scene.Player.LastNoteText
                .Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries))
            {
                textRenderer.RenderText(line, black, screen, currentY, noteStartX);
                currentY += lineHeight;
            }
        }
    }
}