using System;
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
            var sceneRenderer = new SceneRenderer(Sprite.Load("./sprites/brick_wall.png"));

            return new Engine(screen, controller, eventsCycle, sceneRenderer);
        }

        public void Run(Level level)
        {
            eventsCycle.AddListener(controller);
            controller.OnMouseMove(x => OnMouseMove(x, level.Player));
            var previousTime = SDL_GetPerformanceCounter();
            var counterFrequency = SDL_GetPerformanceFrequency() / 1000.0;

            while (true)
            {
                var currentTime = SDL_GetPerformanceCounter();
                var elapsedMilliseconds = (currentTime - previousTime) / counterFrequency;
                previousTime = currentTime;

                eventsCycle.ProcessEvents();

                if (controller.IsKeyPressed(SDL_Keycode.SDLK_q))
                    break;

                screen.Clear();
                ProcessKeyboard(elapsedMilliseconds, level.Map, level.Player);
                sceneRenderer.Render(screen, level, elapsedMilliseconds);
                screen.Update();
            }
        }

        private void OnMouseMove(int x, Player player)
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
                    DoLeftTurn(mouseSensitivity, player);
                }
                else
                {
                    DoRightTurn(mouseSensitivity, player);
                }
            }
            else
            {
                if (x < lastMousePosition)
                {
                    DoLeftTurn(mouseSensitivity, player);
                }
                else
                {
                    DoRightTurn(mouseSensitivity, player);
                }
            }

            lastMousePosition = x;
        }

        private void ProcessKeyboard(double elapsedMs, Map map, Player player)
        {
            if (controller.IsKeyPressed(SDL_Keycode.SDLK_LEFT))
            {
                DoLeftTurn(elapsedMs, player);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_RIGHT))
            {
                DoRightTurn(elapsedMs, player);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_w))
            {
                var dx = Math.Sin(player.ViewAngle) * player.MovingSpeed * elapsedMs;
                var dy = Math.Cos(player.ViewAngle) * player.MovingSpeed * elapsedMs;

                TryMove(dx, dy, map, player);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_s))
            {
                var dx = Math.Sin(player.ViewAngle) * player.MovingSpeed * elapsedMs;
                var dy = Math.Cos(player.ViewAngle) * player.MovingSpeed * elapsedMs;

                TryMove(-dx, -dy, map, player);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_a))
            {
                var dx = Math.Cos(player.ViewAngle) * player.MovingSpeed * elapsedMs;
                var dy = Math.Sin(player.ViewAngle) * player.MovingSpeed * elapsedMs;

                TryMove(-dx, dy, map, player);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_d))
            {
                var dx = Math.Cos(player.ViewAngle) * player.MovingSpeed * elapsedMs;
                var dy = Math.Sin(player.ViewAngle) * player.MovingSpeed * elapsedMs;

                TryMove(dx, -dy, map, player);
            }
        }

        private void TryMove(double dx, double dy, Map map, Player player)
        {
            var newX = player.X + dx;
            var newY = player.Y + dy;
            var testX = (int) newX;
            var testY = (int) newY;

            if (map.InBound(testY, testX) && map.At(testY, testX) != Cell.Wall)
            {
                player.X = newX;
                player.Y = newY;
            }
        }

        private static void DoLeftTurn(double elapsed, Player player)
        {
            player.ViewAngle -= 0.003 * elapsed;
        }

        private static void DoRightTurn(double elapsed, Player player)
        {
            player.ViewAngle += 0.003 * elapsed;
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