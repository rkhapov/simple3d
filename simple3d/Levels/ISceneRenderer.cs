using System;
using simple3d.Ui;

namespace simple3d.Levels
{
    public interface ISceneRenderer: IDisposable
    {
        void Render(IScreen screen, Scene scene, float elapsedMilliseconds, bool renderMap);
    }
}