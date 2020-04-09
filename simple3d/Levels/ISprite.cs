using System;

namespace simple3d.Levels
{
    public interface ISprite : IDisposable
    {
        int Height { get; }
        int Width { get; }
        int GetSample(float y, float x);
    }
}