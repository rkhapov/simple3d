using System;

namespace simple3d.Levels
{
    public class Monologue
    {
        private readonly string[] texts;
        private readonly int[] durations;
        private int currentText;
        private float currentTime;

        public Monologue(string[] texts, int[] durations)
        {
            this.texts = texts;
            this.durations = durations;
            if (texts.Length != durations.Length)
            {
                throw new InvalidOperationException($"durations length != texts length {durations.Length} != {texts.Length}");
            }
            currentText = 0;
            currentTime = 0.0f;
        }

        public string CurrentText => texts[currentText];

        public bool IsOver => currentText >= texts.Length;

        public void Update(float elapsedMilliseconds)
        {
            currentTime += elapsedMilliseconds;

            if (currentTime > durations[currentText])
            {
                currentText++;
                currentTime = 0.0f;
            }
        }
    }
}