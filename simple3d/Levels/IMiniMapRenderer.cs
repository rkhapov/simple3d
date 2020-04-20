using System;
using simple3d.Ui;

namespace simple3d.Levels
{
    public interface IMiniMapRenderer : IDisposable
    {
        void Render(IScreen screen, Scene scene);
    }
}