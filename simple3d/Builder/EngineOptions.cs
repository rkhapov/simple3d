namespace simple3d.Builder
{
    public class EngineOptions
    {
        public EngineOptions(
            string windowTitle,
            int screenHeight,
            int screenWidth,
            bool fullScreen, string fontPath = null, string crossSpritePath = null,
            string notesSpritePath = null)
        {
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
            FullScreen = fullScreen;
            NotesSpritePath = notesSpritePath;
            CrossSpritePath = crossSpritePath;
            FontPath = fontPath;
            WindowTitle = windowTitle;
        }

        public int ScreenHeight { get; }
        public int ScreenWidth { get; }
        public string WindowTitle { get; }
        public bool FullScreen { get; }
        public string FontPath { get; }
        public string CrossSpritePath { get; }
        public string NotesSpritePath { get; }
    }
}