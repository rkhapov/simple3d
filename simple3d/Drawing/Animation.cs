using System.IO;
using System.Linq;

namespace simple3d.Drawing
{
    public class Animation
    {
        private readonly Sprite[] frames;
        private readonly float frameDurationMilliseconds;
        private float currentTime;
        private int currentFrame;

        public Animation(Sprite[] frames, float frameDurationMilliseconds)
        {
            this.frames = frames;
            this.frameDurationMilliseconds = frameDurationMilliseconds;
            currentTime = 0.0f;
            currentFrame = 0;
        }

        public void UpdateFrame(float elapsedMilliseconds)
        {
            currentTime += elapsedMilliseconds;
            if (currentTime > frameDurationMilliseconds)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                currentTime = 0.0f;
            }
        }

        public void Reset()
        {
            currentTime = 0.0f;
            currentFrame = 0;
        }

        public Sprite CurrentFrame => frames[currentFrame];

        public static Animation LoadFromDirectory(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            var frames = directoryInfo
                .GetFiles("*.png")
                .OrderBy(f => f.Name)
                .Select(f => Sprite.Load(f.FullName))
                .ToArray();

            return new Animation(frames, 500);
        }
    }
}