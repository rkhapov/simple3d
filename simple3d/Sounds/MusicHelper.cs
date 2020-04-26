using simple3d.SDL2;

namespace simple3d.Sounds
{
    public static class MusicHelper
    {
        public static bool PlayingMusic()
        {
            return SDL_mixer.Mix_PlayingMusic() != 0;
        }

        public static void StopPlaying()
        {
            SDL_mixer.Mix_HaltMusic();
        }
    }
}