using System.Collections.Generic;

namespace simple3d.Scene
{
    public enum Cell
    {
        Empty,
        Wall,
        Skeleton
    }

    public class Skeleton
    {
        public Skeleton(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }
        public float Y { get; }
    }

    public class Map
    {
        private readonly Cell[,] map;

        public int Height { get; }
        public int Width { get; }

        private Map(Cell[,] map, int height, int width)
        {
            this.map = map;
            Height = height;
            Width = width;
        }

        public static Map FromStrings(string[] strings)
        {
            var height = strings.Length;
            var width = strings[0].Length;
            var map = new Cell[height, width];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    map[i, j] = GetCellByChar(strings[i][j]);
                }
            }

            return new Map(map, height, width);
        }

        public IEnumerable<Skeleton> GetSkeletons()
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    if (map[i, j] == Cell.Skeleton)
                    {
                        yield return new Skeleton(j + 0.5f, i + 0.5f);
                    }
                } 
            }
        }

        private static Cell GetCellByChar(char c)
        {
            return c switch
            {
                '#' => Cell.Wall,
                '@' => Cell.Skeleton,
                _ => Cell.Empty
            };
        }

        public bool InBound(int y, int x)
        {
            return y >= 0 && y < Height && x >= 0 && x < Width;
        }

        public Cell At(int y, int x)
        {
            return map[y, x];
        }
    }
}