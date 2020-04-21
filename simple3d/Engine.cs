using System;
using System.Runtime.CompilerServices;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Events;
using simple3d.Levels;
using simple3d.SDL2;
using simple3d.Ui;
using static simple3d.SDL2.SDL;

namespace simple3d
{
    public class Engine : IEngine
    {
        private readonly ISceneRenderer sceneRenderer;
        private readonly IMiniMapRenderer miniMapRenderer;
        private readonly IStatusBarRenderer statusBarRenderer;
        private ulong previousTime;
        private readonly float counterFrequency;
        private IController Controller { get; }
        private IScreen Screen { get; }
        private IEventsCycle EventsCycle { get; }

        private Engine(
            IScreen screen,
            IController controller,
            IEventsCycle eventsCycle,
            ISceneRenderer sceneRenderer,
            IMiniMapRenderer miniMapRenderer,
            IStatusBarRenderer statusBarRenderer)
        {
            Screen = screen;
            Controller = controller;
            EventsCycle = eventsCycle;
            this.sceneRenderer = sceneRenderer;
            this.miniMapRenderer = miniMapRenderer;
            this.statusBarRenderer = statusBarRenderer;

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

            var screen = Ui.Screen.Create(options.WindowTitle, options.ScreenHeight, options.ScreenWidth, options.FullScreen);
            var miniMapRenderer = new MiniMapRenderer();
            var statusBarHeight = screen.Height / 8;
            var statusBarWidth = screen.Width;
            var statusBarSprite = NoiseSpriteGenerator.GenerateSmoothedNoiseSprite(statusBarHeight, statusBarWidth);
            var statusBarRenderer = new StatusRenderer(statusBarSprite, statusBarHeight);

            return new Engine(screen, controller, eventsCycle, sceneRenderer, miniMapRenderer, statusBarRenderer);
        }

        public bool Update(Scene scene)
        {
            var currentTime = SDL_GetPerformanceCounter();
            var elapsedMilliseconds = (currentTime - previousTime) / counterFrequency;
            previousTime = currentTime;

            if (!ProcessEvents(scene, elapsedMilliseconds))
                return false;

            UpdateWorld(scene, elapsedMilliseconds);

            Render(scene, elapsedMilliseconds);

            // Console.WriteLine($"FPS = {1000 / elapsedMilliseconds}");

            return true;
        }

        private bool ProcessEvents(Scene scene, float elapsedMilliseconds)
        {
            EventsCycle.ProcessEvents();

            if (EventsCycle.ExitRequested)
                return false;

            //TODO: remove this
            if (Controller.IsKeyPressed(SDL_Keycode.SDLK_q))
                return false;

            foreach (var playerAction in Controller.GetCurrentPlayerActions())
            {
                scene.Player.ProcessAction(playerAction, scene, elapsedMilliseconds);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateWorld(Scene scene, float elapsedMilliseconds)
        {
            foreach (var mapObject in scene.Objects)
            {
                mapObject.OnWorldUpdate(scene, elapsedMilliseconds);
            }

            scene.Player.OnWorldUpdate(scene, elapsedMilliseconds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Render(Scene scene, float elapsedMilliseconds)
        {
            Screen.Clear();
            sceneRenderer.Render(Screen, scene, elapsedMilliseconds);

            statusBarRenderer.Render(Screen, scene);

            if (Controller.IsKeyPressed(SDL_Keycode.SDLK_m))
                miniMapRenderer.Render(Screen, scene);

            statusBarRenderer.Render(Screen, scene);

            Screen.Update();
        }

        public void Dispose()
        {
            sceneRenderer.Dispose();
            Screen.Dispose();
            SDL_image.IMG_Quit();
            SDL_Quit();
        }
    }
}