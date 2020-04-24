using System.Collections.Concurrent;
using simple3d.Drawing;
using simple3d.Sounds;

namespace simple3d
{
    public class ResourceCachedLoader
    {
        private readonly ConcurrentDictionary<string, Sprite> nameToSpriteCache
            = new ConcurrentDictionary<string, Sprite>();
        private readonly ConcurrentDictionary<string, Animation> nameToAnimationCache
            = new ConcurrentDictionary<string, Animation>();
        private readonly ConcurrentDictionary<string, ISound> nameToSoundCache
            = new ConcurrentDictionary<string, ISound>();
        private readonly ConcurrentDictionary<string, IMusic> nameToMusicCache
            = new ConcurrentDictionary<string, IMusic>();

        public Sprite GetSprite(string path)
        {
            if (nameToSpriteCache.TryGetValue(path, out var sprite))
                return sprite;

            return nameToSpriteCache[path] = Sprite.Load(path);
        }

        public Animation GetAnimation(string directoryPath)
        {
            if (nameToAnimationCache.TryGetValue(directoryPath, out var animation))
                return animation;

            return nameToAnimationCache[directoryPath] = Animation.LoadFromDirectory(directoryPath);
        }

        public ISound GetSound(string path)
        {
            if (nameToSoundCache.TryGetValue(path, out var sound))
            {
                return sound;
            }

            return nameToSoundCache[path] = Sound.Load(path);
        }

        public IMusic GetMusic(string path)
        {
            if (nameToMusicCache.TryGetValue(path, out var music))
            {
                return music;
            }

            return nameToMusicCache[path] = Music.Load(path);
        }
    }
}