using System;
using simple3d.SDL2;

namespace simple3d.Sounds
{
    public class Sound : ISound
    {
        private readonly IntPtr chunk;

        public Sound(IntPtr chunk)
        {
            this.chunk = chunk;
        }

        public static Sound Load(string path)
        {
            var chunk = SDL_mixer.Mix_LoadWAV(path);

            if (chunk == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Cant load wav '{path}': {SDL.SDL_GetError()}");
            }

            return new Sound(chunk);
        }

        public void Play(int loopsCount)
        {
            if (SDL_mixer.Mix_PlayChannel(-1, chunk, loopsCount) < 0)
            {
                throw new NotImplementedException($"Cant play chunk: {SDL.SDL_GetError()}");
            }
        }

        public void Dispose()
        {
            SDL_mixer.Mix_FreeChunk(chunk);
        }
    }
}