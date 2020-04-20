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