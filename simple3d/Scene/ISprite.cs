using System;

namespace simple3d.Scene
{
    public interface ISprite : IDisposable
    {
        int Height { get; }
        int Width { get; }
        int GetSample(double y, double x);
    }
}