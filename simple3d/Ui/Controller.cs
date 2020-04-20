using System.Collections.Generic;
using simple3d.Levels;
using simple3d.SDL2;

namespace simple3d.Ui
{
    public class Controller : IController
    {
        private readonly Dictionary<SDL.SDL_Keycode, bool> keycodeToState;

        public Controller()
        {
            keycodeToState = new Dictionary<SDL.SDL_Keycode, bool>();
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
            }
        }

        public IEnumerable<PlayerAction> GetCurrentPlayerActions()
        {
            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_LEFT))
            {
                yield return PlayerAction.LeftCameraTurn;
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                yield return PlayerAction.RightCameraTurn;
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_w))
            {
                yield return PlayerAction.MoveForward;
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_s))
            {
                yield return PlayerAction.MoveBackward;
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_a))
            {
                yield return PlayerAction.MoveLeft;
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_d))
            {
                yield return PlayerAction.MoveRight;
            }
        }
    }
}