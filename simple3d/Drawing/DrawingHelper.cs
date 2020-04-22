using System;
using simple3d.SDL2;

namespace simple3d.Drawing
{
    public static class DrawingHelper
    {
        public static unsafe Sprite GetSpriteFromSdlSurface(IntPtr surfacePtr)
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
                    var pixel = *(uint*) ((byte*) pixels + i * pitch + j * bytesPerPixel);
                    pixel = pixel & 0xFF00FF00 | ((pixel & 0xFF) << 16) | ((pixel & 0xFF0000) >> 16);
                    buffer[i * width + j] = (int) pixel;
                }
            }

            return new Sprite(buffer, height, width);
        }

        public static unsafe void RestoreRenderedText(Sprite rendered, SDL.SDL_Color textColor)
        {
            
        }
    }
}