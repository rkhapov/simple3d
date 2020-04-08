using System;

namespace simple3d.Ui
{
    public interface IScreen: IDisposable
    {
        int Height { get; }
        int Width { get; }
        void Draw(int y, int x, byte r, byte g, byte b);
        void Update();
        void Clear();
    }
}