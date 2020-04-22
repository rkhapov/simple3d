using System;
using System.Runtime.CompilerServices;
using simple3d.Levels;
using simple3d.SDL2;

namespace simple3d.Drawing
{
    public class Sprite : ISprite
    {
        private readonly int[] buffer;

        public int[] GetRawBuffer() => buffer;

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

            var sprite = DrawingHelper.GetSpriteFromSdlSurface(surface);

            SDL.SDL_UnlockSurface(surface);
            SDL.SDL_FreeSurface(surface);

            return sprite;
        }

        public void Dispose()
        {
        }

        public int Height { get; }

        public int Width { get; }
        public readonly float AspectRatio;
    }
}