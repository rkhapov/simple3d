using System;

namespace simple3d.Drawing
{
    public static class NoiseSpriteGenerator
    {
        private static readonly Random random = new Random();

        public static Sprite GenerateSmoothedNoiseSprite(int height, int width, int turbulenceCounter = 64)
        {
            var noise = GenerateNoiseArray(height, width);
            var buffer = new int[height * width];

            for (var y0 = 0; y0 < height; y0++)
            {
                var y = y0 * width;

                for (var x = 0; x < width; x++)
                {
                    var b = (int) Turbulence(noise, x, y0, turbulenceCounter, width, height);
                    var color = b << 16 | b << 8 | b;
                    buffer[y + x] = color;
                }
            }

            return Sprite.FromBuffer(buffer, height, width);
        }

        private static double[,] GenerateNoiseArray(int height, int width)
        {
            var noise = new double[height, width];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    noise[y, x] = random.NextDouble();
                }
            }

            return noise;
        }

        private static double GetSmoothedNoise(double[,] noise, double x, double y, int width, int height)
        {
            var fractionalX = x - (int) x;
            var fractionalY = y - (int) y;

            var x1 = ((int) x + width) % width;
            var y1 = ((int) y + height) % height;

            var x2 = (x1 + width - 1) % width;
            var y2 = (y1 + height - 1) % height;

            var value = 0.0;
            value += fractionalX * fractionalY * noise[y1, x1];
            value += (1 - fractionalX) * fractionalY * noise[y1, x2];
            value += fractionalX * (1 - fractionalY) * noise[y2, x1];
            value += (1 - fractionalX) * (1 - fractionalY) * noise[y2, x2];

            return value;
        }

        private static double Turbulence(double[,] noise, double x, double y, double size, int width, int height)
        {
            var value = 0.0;
            var initialSize = size;

            while (size > 0.9)
            {
                value += GetSmoothedNoise(noise, x / size, y / size, width, height) * size;
                size /= 2.0;
            }

            return 128 * value / initialSize;
        }
    }
}