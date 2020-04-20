using System;
using System.Collections.Generic;
using simple3d.Levels;
using simple3d.SDL2;

namespace simple3d.Ui
{
    public class Controller : IController
    {
        private readonly Dictionary<SDL.SDL_Keycode, bool> keycodeToState;
        private readonly Queue<(SDL.SDL_Keycode keycode, bool isDown)> eventsQueue;

        public Controller()
        {
            keycodeToState = new Dictionary<SDL.SDL_Keycode, bool>();
            eventsQueue = new Queue<(SDL.SDL_Keycode keycode, bool isDown)>();
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
                    eventsQueue.Enqueue((@event.key.keysym.sym, true));
                    break;
                case SDL.SDL_EventType.SDL_KEYUP:
                    keycodeToState[@event.key.keysym.sym] = false;
                    eventsQueue.Enqueue((@event.key.keysym.sym, false));
                    break;
            }
        }

        public IEnumerable<PlayerAction> GetCurrentPlayerActions()
        {
            while (eventsQueue.Count != 0)
            {
                var (keycode, isPressed) = eventsQueue.Dequeue();

                if (keycode == SDL.SDL_Keycode.SDLK_LEFT)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.LeftCameraTurn);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_RIGHT)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.RightCameraTurn);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_w)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.MoveForward);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_s)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.MoveBackward);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_a)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.MoveLeft);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_d)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.MoveRight);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_v)
                {
                    yield return IsKeyPressed(SDL.SDL_Keycode.SDLK_RSHIFT)
                        ? new PlayerAction(isPressed, PlayerActionType.MeleeLeftBlock)
                        : new PlayerAction(isPressed, PlayerActionType.MeleeLeftAttack);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_b)
                {
                    yield return IsKeyPressed(SDL.SDL_Keycode.SDLK_RSHIFT)
                        ? new PlayerAction(isPressed, PlayerActionType.MeleeRightBlock)
                        : new PlayerAction(isPressed, PlayerActionType.MeleeRightAttack);
                }
            }
        }
    }
}