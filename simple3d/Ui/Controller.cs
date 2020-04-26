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

                if (keycode == SDL.SDL_Keycode.SDLK_LSHIFT)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.Sprint);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_RSHIFT)
                {
                    if (IsKeyPressed(SDL.SDL_Keycode.SDLK_SPACE))
                        yield return new PlayerAction(isPressed, PlayerActionType.MeleeRightBlock);
                    else if (IsKeyPressed(SDL.SDL_Keycode.SDLK_LCTRL))
                        yield return new PlayerAction(isPressed, PlayerActionType.MeleeLeftBlock);
                    else
                        yield return new PlayerAction(false, PlayerActionType.MeleeLeftBlock);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_RETURN)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.SetWeapon);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_SPACE)
                {
                    yield return new PlayerAction(isPressed, PlayerActionType.Shoot);

                    yield return IsKeyPressed(SDL.SDL_Keycode.SDLK_RSHIFT)
                        ? new PlayerAction(isPressed, PlayerActionType.MeleeRightBlock)
                        : new PlayerAction(isPressed, PlayerActionType.MeleeRightAttack);
                }

                if (keycode == SDL.SDL_Keycode.SDLK_LCTRL)
                {
                    yield return IsKeyPressed(SDL.SDL_Keycode.SDLK_RSHIFT)
                        ? new PlayerAction(isPressed, PlayerActionType.MeleeLeftBlock)
                        : new PlayerAction(isPressed, PlayerActionType.MeleeLeftAttack);
                }
                
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_LEFT))
            {
                yield return new PlayerAction(true, PlayerActionType.CameraTurnLeft);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                yield return new PlayerAction(true, PlayerActionType.CameraTurnRight);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_w))
            {
                yield return new PlayerAction(true, PlayerActionType.MoveForward);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_s))
            {
                yield return new PlayerAction(true, PlayerActionType.MoveBackward);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_a))
            {
                yield return new PlayerAction(true, PlayerActionType.MoveLeft);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_d))
            {
                yield return new PlayerAction(true, PlayerActionType.MoveRight);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_e))
            {
                yield return new PlayerAction(true, PlayerActionType.Interact);
            }

            if (IsKeyPressed(SDL.SDL_Keycode.SDLK_ESCAPE))
            {
                yield return new PlayerAction(true, PlayerActionType.Cancel);
            }
        }
    }
}