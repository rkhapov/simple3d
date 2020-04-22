namespace simple3d.Builder
{
    public class EngineOptions
    {
        public EngineOptions(
            string windowTitle,
            int screenHeight,
            int screenWidth,
            bool fullScreen,
            string fontPath = null)
        {
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
            FullScreen = fullScreen;
            FontPath = fontPath;
            WindowTitle = windowTitle;
        }

        public int ScreenHeight { get; }
        public int ScreenWidth { get; }
        public string WindowTitle { get; }
        public bool FullScreen { get; }
        public string FontPath { get; }
    }
}