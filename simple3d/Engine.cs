using System;
using System.Runtime.CompilerServices;
using simple3d.Builder;
using simple3d.Events;
using simple3d.Scene;
using simple3d.SDL2;
using simple3d.Ui;
using static simple3d.SDL2.SDL;

namespace simple3d
{
    public class Engine : IEngine
    {
        private readonly IController controller;
        private readonly IEventsCycle eventsCycle;
        private readonly IScreen screen;
        private readonly ISceneRenderer sceneRenderer;

        private int lastMousePosition;

        private Engine(IScreen screen, IController controller, IEventsCycle eventsCycle, ISceneRenderer sceneRenderer)
        {
            this.screen = screen;
            this.controller = controller;
            this.eventsCycle = eventsCycle;
            this.sceneRenderer = sceneRenderer;
            lastMousePosition = -1;
        }

        public static Engine Create(EngineOptions options)
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                throw new InvalidOperationException($"Cant initialize SDL2: {SDL_GetError()}");
            }

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG) < 0)
            {
                throw new InvalidOperationException($"Cant initialize SDL_image: {SDL_GetError()}");
            }

            if (SDL_ShowCursor(0) < 0)
            {
                throw new InvalidOperationException($"Cant disable cursor: {SDL_GetError()}");
            }

            var screen = Screen.Create(options.WindowTitle, options.ScreenHeight, options.ScreenWidth);
            var controller = new Controller();
            var eventsCycle = new EventsCycle();
            var sceneRenderer = new SceneRenderer();

            return new Engine(screen, controller, eventsCycle, sceneRenderer);
        }

        public void RunLevel(Level level)
        {
            eventsCycle.AddListener(controller);
            controller.OnMouseMove(x => OnMouseMove(x, level.PlayerCamera));
            var previousTime = SDL_GetPerformanceCounter();
            var counterFrequency = SDL_GetPerformanceFrequency() / 1000.0f;

            while (true)
            {
                var currentTime = SDL_GetPerformanceCounter();
                var elapsedMilliseconds = (currentTime - previousTime) / counterFrequency;
                previousTime = currentTime;

                eventsCycle.ProcessEvents();

                if (controller.IsKeyPressed(SDL_Keycode.SDLK_q))
                    break;

                screen.Clear();
                ProcessKeyboard(elapsedMilliseconds, level.Map, level.PlayerCamera);
                sceneRenderer.Render(screen, level, elapsedMilliseconds);
                screen.Update();
            }
        }

        private void OnMouseMove(int x, PlayerCamera playerCamera)
        {
            if (lastMousePosition < 0)
            {
                lastMousePosition = x;
                return;
            }

            const int mouseSensitivity = 25;
            if (x == lastMousePosition)
            {
                if (x == 0)
                {
                    DoLeftTurn(mouseSensitivity, playerCamera);
                }
                else
                {
                    DoRightTurn(mouseSensitivity, playerCamera);
                }
            }
            else
            {
                if (x < lastMousePosition)
                {
                    DoLeftTurn(mouseSensitivity, playerCamera);
                }
                else
                {
                    DoRightTurn(mouseSensitivity, playerCamera);
                }
            }

            lastMousePosition = x;
        }

        private void ProcessKeyboard(float elapsedMs, Map map, PlayerCamera playerCamera)
        {
            if (controller.IsKeyPressed(SDL_Keycode.SDLK_LEFT))
            {
                DoLeftTurn(elapsedMs, playerCamera);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_RIGHT))
            {
                DoRightTurn(elapsedMs, playerCamera);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_w))
            {
                var dx = MathF.Sin(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;
                var dy = MathF.Cos(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;

                TryMove(dx, dy, map, playerCamera);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_s))
            {
                var dx = MathF.Sin(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;
                var dy = MathF.Cos(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;

                TryMove(-dx, -dy, map, playerCamera);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_a))
            {
                var dx = MathF.Cos(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;
                var dy = MathF.Sin(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;

                TryMove(-dx, dy, map, playerCamera);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_d))
            {
                var dx = MathF.Cos(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;
                var dy = MathF.Sin(playerCamera.ViewAngle) * playerCamera.MovingSpeed * elapsedMs;

                TryMove(dx, -dy, map, playerCamera);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryMove(float dx, float dy, Map map, PlayerCamera playerCamera)
        {
            var newX = playerCamera.X + dx;
            var newY = playerCamera.Y + dy;
            var testX = (int) newX;
            var testY = (int) newY;

            if (map.InBound(testY, testX) && map.At(testY, testX).Type != MapCellType.Wall)
            {
                playerCamera.X = newX;
                playerCamera.Y = newY;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DoLeftTurn(float elapsed, PlayerCamera playerCamera)
        {
            playerCamera.ViewAngle -= 0.003f * elapsed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DoRightTurn(float elapsed, PlayerCamera playerCamera)
        {
            playerCamera.ViewAngle += 0.003f * elapsed;
        }

        public void Dispose()
        {
            sceneRenderer.Dispose();
            screen.Dispose();
            SDL_image.IMG_Quit();
            SDL_Quit();
        }
    }
}