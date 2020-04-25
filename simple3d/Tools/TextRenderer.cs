using System;
using simple3d.Drawing;
using simple3d.SDL2;
using simple3d.Ui;

namespace simple3d.Tools
{
    public class TextRenderer : ITextRenderer
    {
        private readonly IntPtr font;

        private TextRenderer(IntPtr font)
        {
            this.font = font;
        }

        public static TextRenderer Load(string fontPath, int size)
        {
            var font = SDL_ttf.TTF_OpenFont(fontPath, size);

            if (font == IntPtr.Zero)
                throw new InvalidOperationException($"Cant load font {fontPath}: {SDL.SDL_GetError()}");

            return new TextRenderer(font);
        }
        
        public Sprite RenderText(string text, SDL.SDL_Color color)
        {
            var surface = SDL_ttf.TTF_RenderUTF8_Blended(font, text, color);
            var sprite = DrawingHelper.GetSpriteFromSdlSurface(surface);

            SDL.SDL_FreeSurface(surface);

            return sprite;
        }

        public void RenderText(string text, SDL.SDL_Color color, IScreen screen, int y, int x)
        {
            var sprite = RenderText(text, color);
            screen.DrawSprite(sprite, y, x);
        }

        public void Dispose()
        {
            SDL_ttf.TTF_CloseFont(font);
        }
    }
}