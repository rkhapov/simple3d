using simple3d.Drawing;
using simple3d.Tools;
using simple3d.Ui;

namespace simple3d.Levels
{
    public class NotesRenderer : INotesRenderer
    {
        private readonly Sprite noteSprite;
        private readonly ITextRenderer textRenderer;
        private readonly int statusBarHeight;

        public NotesRenderer(Sprite noteSprite, ITextRenderer textRenderer, int statusBarHeight)
        {
            this.noteSprite = noteSprite;
            this.textRenderer = textRenderer;
            this.statusBarHeight = statusBarHeight;
        }

        public void Render(IScreen screen, Scene scene)
        {
            if (scene.Player.State != Player.PlayerState.TextReading)
            {
                return;
            }

            var visiblePartHeight = screen.Height - statusBarHeight;
            var height = (int) (visiblePartHeight * 0.9f);
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
            
            // textRenderer.RenderText()
        }
    }
}