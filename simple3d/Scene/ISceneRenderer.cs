using System;
using simple3d.Ui;

namespace simple3d.Scene
{
    public interface ISceneRenderer: IDisposable
    {
        void Render(IScreen screen, Level level, float elapsedMilliseconds);
    }
}