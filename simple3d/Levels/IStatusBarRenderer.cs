using System;
using simple3d.Ui;

namespace simple3d.Levels
{
    public interface IStatusBarRenderer : IDisposable
    {
        void Render(IScreen screen, Scene scene);
    }
}