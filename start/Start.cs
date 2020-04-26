using System;
using simple3d.Builder;
using ui;
using menu;

namespace start
{
    class Start
    {
        static void Main(string[] args)
        {
            var engine = EngineBuilder.BuildEngine25D(
                new EngineOptions("simple 3d game", 720, 1280,
                    false,
                    UiResourcesHelper.PressStart2PFontPath,
                    UiResourcesHelper.CrossSpritePath,
                    UiResourcesHelper.ScrollSpritePath));
            Menu.StartOnEngine(engine);
        }
    }
}