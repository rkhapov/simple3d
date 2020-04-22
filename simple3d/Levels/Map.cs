using System;
using simple3d.Drawing;

namespace simple3d.Levels
{
    public class Map
    {
        private readonly MapCell[] map;

        public int Height { get; }
        public int Width { get; }

        private Map(MapCell[] map, int height, int width)
        {
            this.map = map;
            Height = height;
            Width = width;
        }

        public static Map FromStrings(string[] strings, Func<char, MapCell> cellFactory)
        {
            var height = strings.Length;
            var width = strings[0].Length;
            var map = new MapCell[height * width];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    map[i * width + j] = cellFactory(strings[i][j]);
                }
            }

            return new Map(map, height, width);
        }

        public bool InBound(int y, int x)
        {
            return y >= 0 && y < Height && x >= 0 && x < Width;
        }

        public MapCell At(int y, int x)
        {
            //TODO: speed up this shit, we will assume that before every
            //cell acessing user will call InBound
            return map[y * Width + x];
        }
    }
}