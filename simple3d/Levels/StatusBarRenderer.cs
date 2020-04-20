using simple3d.Ui;

namespace simple3d.Levels
{
    public class StatusBarRenderer : IStatusBarRenderer
    {
        public void Dispose()
        {
            //nothing
        }

        public void Render(IScreen screen, Scene scene)
        {
            var screenHeight = screen.Height;
            var screenWidth = screen.Width;
            var startY = screen.Height - screen.Height / 8;

            for (var y = startY; y < screenHeight; y++)
            {
                for (var x = 0; x < screenWidth; x++)
                {
                    screen.Draw(y, x, 0, 0x64, 0);
                }
            }
        }
    }
}