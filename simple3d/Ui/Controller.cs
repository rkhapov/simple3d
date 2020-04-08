using System;
using System.Collections.Generic;
using simple3d.Events;
using simple3d.SDL2;

namespace simple3d.Ui
{
    public class Controller : IController, IEventsListener
    {
        private readonly List<Action<int>> mouseHandlers;
        private readonly Dictionary<SDL.SDL_Keycode, bool> keycodeToState;

        public Controller()
        {
            mouseHandlers = new List<Action<int>>();
            keycodeToState = new Dictionary<SDL.SDL_Keycode, bool>();
        }

        public void OnMouseMove(Action<int> handler)
        {
            mouseHandlers.Add(handler);
        }

        public bool IsKeyPressed(SDL.SDL_Keycode keycode)
        {
            return keycodeToState.TryGetValue(keycode, out var state) && state;
        }

        public void HandleEvent(SDL.SDL_Event @event)
        {
            switch (@event.type)
            {
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    keycodeToState[@event.key.keysym.sym] = true;
                    break;
                case SDL.SDL_EventType.SDL_KEYUP:
                    keycodeToState[@event.key.keysym.sym] = false;
                    break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    foreach (var mouseHandler in mouseHandlers)
                    {
                        mouseHandler(@event.motion.x);
                    }
                    break;
            }
        }
    }
}