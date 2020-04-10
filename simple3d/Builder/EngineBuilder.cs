using simple3d.Events;
using simple3d.Levels;
using simple3d.Ui;

namespace simple3d.Builder
{
    public static class EngineBuilder
    {
        public static IEngine BuildEngine(EngineOptions options, IController controller, IEventsCycle eventsCycle, ISceneRenderer sceneRenderer)
        {
            return Engine.Create(options, controller, eventsCycle, sceneRenderer);
        }

        public static IEngine BuildEngine25D(EngineOptions options)
        {
            return Engine.Create(options, new Controller(), new EventsCycle(), new Scene25DRenderer());
        }
    }
}