using System;
using simple3d.Drawing;
using simple3d.SDL2;
using simple3d.Ui;

namespace simple3d.Tools
{
    public interface ITextRenderer: IDisposable
    {
        Sprite RenderText(string text, SDL.SDL_Color color);
        void RenderText(string text, SDL.SDL_Color color, IScreen screen, int y, int x);
    }
}