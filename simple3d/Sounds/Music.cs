using System;
using simple3d.SDL2;

namespace simple3d.Sounds
{
    public class Music : IMusic
    {
        private readonly IntPtr sdlMusic;

        private Music(IntPtr sdlMusic)
        {
            this.sdlMusic = sdlMusic;
        }

        public static Music Load(string path)
        {
            var sdlMusic = SDL_mixer.Mix_LoadMUS(path);

            if (sdlMusic == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Cant load music '{path}': {SDL.SDL_GetError()}");
            }
            
            return new Music(sdlMusic);
        }

        public void Dispose()
        {
            SDL_mixer.Mix_FreeMusic(sdlMusic);
        }

        public void Play(int loopsCount)
        {
            if (SDL_mixer.Mix_PlayMusic(sdlMusic, loopsCount) < 0)
            {
                throw new InvalidOperationException($"Cant play music: {SDL.SDL_GetError()}");
            }
        }
    }
}