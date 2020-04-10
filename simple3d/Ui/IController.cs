﻿using simple3d.Events;
using simple3d.SDL2;

namespace simple3d.Ui
{
    public interface IController : IEventsListener
    {
        int GetMousePositionX();
        bool IsKeyPressed(SDL.SDL_Keycode keycode);
    }
}