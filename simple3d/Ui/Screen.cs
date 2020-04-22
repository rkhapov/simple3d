using System;
using simple3d.Drawing;
using simple3d.SDL2;
using simple3d.Tools;

namespace simple3d.Ui
{
    public class Screen : IScreen
    {
        private readonly IntPtr window;
        private readonly IntPtr renderer;
        private readonly int[] buffer;
        private readonly IntPtr screenTexture;

        public int Height { get; }
        public int Width { get; }

        private Screen(IntPtr window, IntPtr renderer, IntPtr screenTexture, int height, int width, int[] buffer)
        {
            this.window = window;
            Height = height;
            Width = width;
            this.renderer = renderer;
            this.buffer = buffer;
            this.screenTexture = screenTexture;
        }

        public static Screen Create(string title, int height, int width, bool fullScreen)
        {
            var flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;

            if (fullScreen)
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;

            var window = SDL.SDL_CreateWindow(title,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width, height,
                flags
            );

            if (window == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Cant create screen: {SDL.SDL_GetError()}");
            }

            var renderer = SDL.SDL_CreateRenderer(
                window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if (renderer == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Cant create renderer: {SDL.SDL_GetError()}");
            }

            var screenTexture = SDL.SDL_CreateTexture(renderer,
                SDL.SDL_PIXELFORMAT_ARGB8888,
                (int) SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                width, height);

            if (screenTexture == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Cant create screen texture: {SDL.SDL_GetError()}");
            }

            var buffer = new int[height * width];

            return new Screen(window, renderer, screenTexture, height, width, buffer);
        }

        public unsafe void Draw(int y, int x, byte r, byte g, byte b)
        {
            unchecked
            {
                fixed (int* p = buffer)
                {
                    p[y * Width + x] = 255 << 24 | r << 16 | g << 8 | b;
                }
            }
        }

        public unsafe void Draw(int y, int x, int v)
        {
            fixed (int* p = buffer)
            {
                p[y * Width + x] = v;
            }
        }

        public unsafe void DrawSprite(Sprite sprite, int y, int x)
        {
            var spriteHeight = sprite.Height;
            var spriteWidth = sprite.Width;
            var spriteBuffer = sprite.GetRawBuffer();
            var size = spriteHeight * spriteWidth;
            var lineStep = Width - spriteWidth;

            fixed (int* k = spriteBuffer)
            fixed (int* p = buffer)
            {
                var i = 0;
                var screen = p + y * Width + x;
                var spritePointer = k;

                while (i < size)
                {
                    var pixel = *spritePointer++;
                    if (pixel.IsTransparentColor())
                    {
                        pixel = *screen;
                    }

                    *screen++ = pixel;
                    i++;

                    if (i % spriteWidth == 0)
                    {
                        screen += lineStep;
                    }
                }
            }
        }

        public unsafe void Update()
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(renderer);

            fixed (int* p = buffer)
            {
                SDL.SDL_UpdateTexture(screenTexture, IntPtr.Zero, (IntPtr)p, Width * 4);
                SDL.SDL_RenderCopy(renderer, screenTexture, IntPtr.Zero, IntPtr.Zero);
                SDL.SDL_RenderPresent(renderer);
            }
        }

        public unsafe void Clear()
        {
            fixed (int* p = buffer)
            {
                var pointer = (uint*) p;
                var size = Height * Width;
                var up = pointer + size;
                while (pointer < up)
                {
                    *pointer++ = 0xFF000000;
                }
            }
        }

        public void Dispose()
        {
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyTexture(screenTexture);
            SDL.SDL_DestroyWindow(window);
        }
    }
}