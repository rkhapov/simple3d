namespace simple3d.Builder
{
    public class EngineOptions
    {
        public EngineOptions(string windowTitle, int screenHeight, int screenWidth)
        {
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
            WindowTitle = windowTitle;
        }

        public int ScreenHeight { get; }
        public int ScreenWidth { get; }
        public string WindowTitle { get; }
    }
}