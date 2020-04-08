using simple3d.Ui;

namespace simple3d.Scene
{
    public interface ISceneRenderer
    {
        void Render(IScreen screen, Level level, double elapsedMilliseconds);
    }
}