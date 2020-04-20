using System.Collections.Generic;
using simple3d.Events;
using simple3d.Levels;
using simple3d.SDL2;

namespace simple3d.Ui
{
    public interface IController : IEventsListener
    {
        bool IsKeyPressed(SDL.SDL_Keycode keycode);
        IEnumerable<PlayerAction> GetCurrentPlayerActions();
    }
}