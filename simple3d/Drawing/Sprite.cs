using System;
using System.Runtime.CompilerServices;
using simple3d.Levels;
using simple3d.SDL2;

namespace simple3d.Drawing
{
    public class Sprite : ISprite
    {
        private readonly int[] buffer;

        public Sprite(int[] buffer, int height, int width)
        {
            Height = height;
            Width = width;
            AspectRatio = (float) Height / Width;
            this.buffer = buffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPixel(int y, int x)
        {
            return buffer[y * Width + x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int GetSample(float y, float x)
        {
            var y0 = (int)((y - 1e-6f) * Height);
            var x0 = (int)((x - 1e-6f) * Width);

            //disabling bound checking
            fixed (int* p = buffer)
            {
                return p[y0 * Width + x0];
            }
        }

        public static Sprite FromBuffer(int[] buffer, int height, int width)
        {
            return new Sprite(buffer, height, width);
        }
        
        public static Sprite Load(string path)
        {
            var surface = SDL_image.IMG_Load(path);

            if (surface == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Cant load sprite: {SDL.SDL_GetError()}");
            }

            SDL.SDL_LockSurface(surface);

            var sprite = GetSprite(surface);

            SDL.SDL_UnlockSurface(surface);
            SDL.SDL_FreeSurface(surface);

            return sprite;
        }

        private static unsafe Sprite GetSprite(IntPtr surfacePtr)
        {
            var surface = (SDL.SDL_Surface*) surfacePtr;
            var height = surface->h;
            var width = surface->w;
            var buffer = new int[width * height];
            var pitch = surface->pitch;
            var pixels = surface->pixels;
            var bytesPerPixel = ((SDL.SDL_PixelFormat*) surface->format)->BytesPerPixel;

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixel = *(uint*)((byte*)pixels + i * pitch + j * bytesPerPixel);
                    pixel = pixel & 0xFF00FF00 | ((pixel & 0xFF) << 16) | ((pixel & 0xFF0000) >> 16);
                    buffer[i * width + j] = (int) pixel;
                }
            }

            return new Sprite(buffer, height, width);
        }

        public void Dispose()
        {
        }

        public int Height { get; }

        public int Width { get; }
        public readonly float AspectRatio;
    }
}