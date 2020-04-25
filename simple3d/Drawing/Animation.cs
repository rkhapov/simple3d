using System;
using System.IO;
using System.Linq;

namespace simple3d.Drawing
{
    public class Animation
    {
        private readonly Sprite[] frames;
        private readonly int[] durationOfFrame;
        private float currentTime;
        private int currentFrame;
        private int finishCount = 0;
        private readonly string path;

        private Animation(Sprite[] frames, int[] durationOfFrame, string path)
        {
            this.frames = frames;
            this.durationOfFrame = durationOfFrame;
            this.path = path;
            this.durationOfFrame = durationOfFrame;
            currentTime = 0.0f;
            currentFrame = 0;
        }

        public void UpdateFrame(float elapsedMilliseconds)
        {
            currentTime += elapsedMilliseconds;
            if (currentTime > durationOfFrame[currentFrame])
            {
                currentTime = 0.0f;
                currentFrame++;

                if (currentFrame != frames.Length)
                    return;

                finishCount++;
                currentFrame = 0;
            }
        }

        public void Reset()
        {
            currentTime = 0.0f;
            currentFrame = 0;
            finishCount = 0;
        }

        public Sprite CurrentFrame => frames[currentFrame];

        public Animation GetClearCopy()
        {
            return new Animation(frames, durationOfFrame, path);
        }

        public static Animation FromSingleSprite(Sprite sprite)
        {
            return new Animation(new[] {sprite}, new[] {500}, "<FROM MEMORY>");
        }

        public static Animation LoadFromDirectory(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            var frames = directoryInfo
                .GetFiles("*.png")
                .OrderBy(f => int.Parse(f.Name.Split('.')[0]))
                .Select(f => Sprite.Load(f.FullName))
                .ToArray();
            var durations = LoadDurations(directoryInfo, frames.Length);

            return new Animation(frames, durations, directoryPath);
        }

        private static int[] LoadDurations(DirectoryInfo directoryInfo, int framesCount)
        {
            var durationsFile = directoryInfo.GetFiles("durations.txt");

            if (durationsFile.Length != 1)
                return Enumerable.Repeat(500, framesCount).ToArray();

            var durations = File
                .ReadAllText(durationsFile[0].FullName)
                .Split(new[] {"\n", "\r", " ", "\t"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            if (durations.Length != framesCount)
            {
                throw new InvalidOperationException($"Incorrect durations count: {durations.Length} != {framesCount}");
            }

            return durations;
        }

        public bool IsOver => finishCount > 0 && frames.Length > 1;

        public override string ToString()
        {
            return $"{path} time = {currentTime} frame = {currentFrame} durations = {string.Join(", ", durationOfFrame)}";
        }
    }
}