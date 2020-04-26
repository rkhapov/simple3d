using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Events;
using simple3d.Levels;
using simple3d.SDL2;
using simple3d.Tools;
using simple3d.Ui;
using ui;
using static simple3d.SDL2.SDL;

namespace simple3d
{
    public class Engine : IEngine
    {
        private readonly ISceneRenderer sceneRenderer;
        private readonly IMiniMapRenderer miniMapRenderer;
        private readonly IStatusBarRenderer statusBarRenderer;
        private readonly ITextRenderer textRenderer;
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
            IStatusBarRenderer statusBarRenderer,
            ITextRenderer textRenderer)
        {
            Screen = screen;
            Controller = controller;
            EventsCycle = eventsCycle;
            this.sceneRenderer = sceneRenderer;
            this.miniMapRenderer = miniMapRenderer;
            this.statusBarRenderer = statusBarRenderer;
            this.textRenderer = textRenderer;

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

            if (SDL_ttf.TTF_Init() < 0)
            {
                throw new InvalidOperationException($"TTF_Init: {SDL_GetError()}");
            }

            if (SDL_ShowCursor(0) < 0)
            {
                throw new InvalidOperationException($"Cant disable cursor: {SDL_GetError()}");
            }

            if (SDL_mixer.Mix_Init(
                SDL_mixer.MIX_InitFlags.MIX_INIT_MP3
                | SDL_mixer.MIX_InitFlags.MIX_INIT_MID
                | SDL_mixer.MIX_InitFlags.MIX_INIT_MOD
                | SDL_mixer.MIX_InitFlags.MIX_INIT_OGG
                | SDL_mixer.MIX_InitFlags.MIX_INIT_FLAC
                | SDL_mixer.MIX_InitFlags.MIX_INIT_OPUS) < 0)
            {
                throw new InvalidOperationException($"MixInit: {SDL_GetError()}");
            }

            if (SDL_mixer.Mix_OpenAudio(22050, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 4096) < 0)
            {
                throw new InvalidOperationException($"OpenAudio: {SDL_GetError()}");
            }

            if (SDL_mixer.Mix_AllocateChannels(16) < 0)
            {
                throw new InvalidOperationException($"Cant allocate channels: {SDL_GetError()}");
            }

            if (SDL_mixer.Mix_Volume(-1, 64) < 0)
            {
                throw new InvalidOperationException($"Min_Volume: {SDL_GetError()}");
            }
            

            var screen = Ui.Screen.Create(options.WindowTitle, options.ScreenHeight, options.ScreenWidth,
                options.FullScreen);
            var miniMapRenderer = new MiniMapRenderer();
            var statusBarHeight = screen.Height / 8;
            var statusBarWidth = screen.Width;
            var statusBarSprite = Sprite.Load(UiResourcesHelper.StatusBarSpritePath);
            var bowSprite = Sprite.Load(UiResourcesHelper.BowMiniSpritePath);
            var frameSprite = Sprite.Load(UiResourcesHelper.FrameSpritePath);
            var crossSprite = options.CrossSpritePath == null ? null : Sprite.Load(options.CrossSpritePath);
            var arrowSprite = Sprite.Load(UiResourcesHelper.ArrowSpritePath);
            var swordSprite = Sprite.Load(UiResourcesHelper.SwordSpritePath);
            var fireBallSprite = Sprite.Load(UiResourcesHelper.FireBallSpritePath);
            var shockBallSprite = Sprite.Load(UiResourcesHelper.ShockBallSpritePath);
            var faceSprite = Sprite.Load(UiResourcesHelper.FaceSprintPath);
            var faceHurtedSprite = Sprite.Load(UiResourcesHelper.FaceHurtedSpritePath);
            var faceBadSprite = Sprite.Load(UiResourcesHelper.FaceBadSpritePath);
            var logTextRenderer = TextRenderer.Load(options.FontPath, screen.Height / 50);
            var notesTextRenderer = TextRenderer.Load(options.FontPath, screen.Height / 50);
            var statusTextRenderer = TextRenderer.Load(options.FontPath, screen.Height / 20);
            var notesRenderer = new NotesRenderer(Sprite.Load(options.NotesSpritePath), notesTextRenderer, statusBarHeight);
            var monologueTextRenderer = TextRenderer.Load(options.FontPath, screen.Height / 50);
            var monologueRenderer = new MonologueRenderer(monologueTextRenderer, statusBarHeight);
            var statusBarRenderer = new StatusRenderer(
                statusBarSprite,
                crossSprite,
                statusBarHeight,
                logTextRenderer,
                notesRenderer,
                monologueRenderer,
                bowSprite,
                swordSprite,
                frameSprite,
                statusTextRenderer,
                arrowSprite,
                fireBallSprite,
                shockBallSprite,
                faceSprite,
                faceHurtedSprite,
                faceBadSprite);
            var textRenderer = options.FontPath == null ? null : TextRenderer.Load(options.FontPath, 24);

            return new Engine(screen, controller, eventsCycle, sceneRenderer, miniMapRenderer, statusBarRenderer,
                textRenderer);
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

            return true;
        }

        private bool ProcessEvents(Scene scene, float elapsedMilliseconds)
        {
            EventsCycle.ProcessEvents();

            if (EventsCycle.ExitRequested)
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
            Task.WhenAll(scene
                .Objects
                .Select(obj => Task.Run(() => obj.OnWorldUpdate(scene, elapsedMilliseconds))))
                .Wait();

            scene.Player.OnWorldUpdate(scene, elapsedMilliseconds);

            scene.EventsLogger.Update(elapsedMilliseconds);

            if (scene.Player.CurrentMonologue == null)
                return;

            scene.Player.CurrentMonologue.Update(elapsedMilliseconds);

            if (scene.Player.CurrentMonologue.IsOver)
                scene.Player.CurrentMonologue = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Render(Scene scene, float elapsedMilliseconds)
        {
            Screen.Clear();
            sceneRenderer.Render(Screen, scene, elapsedMilliseconds);

            statusBarRenderer.Render(Screen, scene);

            statusBarRenderer.Render(Screen, scene);

            Screen.Update();
        }

        public void Dispose()
        {
            sceneRenderer.Dispose();
            Screen.Dispose();
            SDL_mixer.Mix_CloseAudio();
            SDL_mixer.Mix_Quit();
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
            SDL_Quit();
        }
    }
}