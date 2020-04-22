using System.Collections.Concurrent;
using simple3d.Drawing;

namespace simple3d
{
    public class ResourceCachedLoader
    {
        private readonly ConcurrentDictionary<string, Sprite> nameToSpriteCache
            = new ConcurrentDictionary<string, Sprite>();
        private readonly ConcurrentDictionary<string, Animation> nameToAnimationCache
            = new ConcurrentDictionary<string, Animation>();

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
    }
}