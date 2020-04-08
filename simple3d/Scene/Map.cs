namespace simple3d.Scene
{
    public enum Cell
    {
        Empty,
        Wall,
        Skeleton
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