using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using simple3d.Builder;
using simple3d.Events;
using simple3d.Levels;
using simple3d.MathUtils;
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
        private ulong previousTime;
        private readonly float counterFrequency;

        private int lastMousePosition;

        private Engine(IScreen screen, IController controller, IEventsCycle eventsCycle, ISceneRenderer sceneRenderer)
        {
            this.screen = screen;
            this.controller = controller;
            this.eventsCycle = eventsCycle;
            this.sceneRenderer = sceneRenderer;
            lastMousePosition = -1;

            previousTime = SDL_GetPerformanceCounter();
            counterFrequency = SDL_GetPerformanceFrequency() / 1000.0f;

            eventsCycle.AddListener(controller);
        }

        public static Engine Create(EngineOptions options, IController controller, IEventsCycle eventsCycle,
            ISceneRenderer sceneRenderer)
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

            return new Engine(screen, controller, eventsCycle, sceneRenderer);
        }

        public bool Update(Scene scene)
        {
            var currentTime = SDL_GetPerformanceCounter();
            var elapsedMilliseconds = (currentTime - previousTime) / counterFrequency;
            previousTime = currentTime;

            eventsCycle.ProcessEvents();

            if (lastMousePosition != controller.GetMousePositionX())
            {
                OnMouseMove(controller.GetMousePositionX(), scene.Player);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_q))
                return false;

            foreach (var mapObject in scene.Objects)
            {
                mapObject.OnWorldUpdate(scene, elapsedMilliseconds);
            }

            screen.Clear();
            ProcessKeyboard(elapsedMilliseconds, scene);
            sceneRenderer.Render(screen, scene, elapsedMilliseconds, controller.IsKeyPressed(SDL_Keycode.SDLK_m));
            screen.Update();
            Console.WriteLine($"FPS: {1000 / elapsedMilliseconds}");

            return true;
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

        private void ProcessKeyboard(float elapsedMs, Scene scene)
        {
            var player = scene.Player;

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
                var dx = MathF.Sin(player.DirectionAngle) * player.MovingSpeed * elapsedMs;
                var dy = MathF.Cos(player.DirectionAngle) * player.MovingSpeed * elapsedMs;

                TryMove(dx, dy, scene);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_s))
            {
                var dx = MathF.Sin(player.DirectionAngle) * player.MovingSpeed * elapsedMs;
                var dy = MathF.Cos(player.DirectionAngle) * player.MovingSpeed * elapsedMs;

                TryMove(-dx, -dy, scene);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_a))
            {
                var dx = MathF.Cos(player.DirectionAngle) * player.MovingSpeed * elapsedMs;
                var dy = MathF.Sin(player.DirectionAngle) * player.MovingSpeed * elapsedMs;

                TryMove(-dx, dy, scene);
            }

            if (controller.IsKeyPressed(SDL_Keycode.SDLK_d))
            {
                var dx = MathF.Cos(player.DirectionAngle) * player.MovingSpeed * elapsedMs;
                var dy = MathF.Sin(player.DirectionAngle) * player.MovingSpeed * elapsedMs;

                TryMove(dx, -dy, scene);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryMove(float dx, float dy, Scene scene)
        {
            //TODO: implement with physics engine?
            var player = scene.Player;
            var map = scene.Map;
            var newPosition = player.Position + new Vector2(dx, 0);

            TryMove(scene, player, newPosition, map);

            newPosition = player.Position + new Vector2(0, dy);

            TryMove(scene, player, newPosition, map);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryMove(Scene scene, Player player, Vector2 newPosition, Map map)
        {
            var playerNewVertices = GeometryHelper.GetRotatedVertices(newPosition, player.Size, player.DirectionAngle);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var vertex in playerNewVertices)
            {
                var testX = (int) vertex.X;
                var testY = (int) vertex.Y;

                if (!map.InBound(testY, testX) || map.At(testY, testX).Type == MapCellType.Wall)
                {
                    return;
                }
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var objectVertices in scene
                .Objects
                .Select(o => o.GetRotatedVertices()))
            {
                if (GeometryHelper.IsRectanglesIntersects(playerNewVertices, objectVertices))
                    return;
            }

            player.Position = newPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DoLeftTurn(float elapsed, Player player)
        {
            player.DirectionAngle -= 0.003f * elapsed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DoRightTurn(float elapsed, Player player)
        {
            player.DirectionAngle += 0.003f * elapsed;
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