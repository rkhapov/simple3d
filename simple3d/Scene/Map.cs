namespace simple3d.Scene
{
    public class Map
    {
        private readonly MapCell[,] map;

        public int Height { get; }
        public int Width { get; }

        private Map(MapCell[,] map, int height, int width)
        {
            this.map = map;
            Height = height;
            Width = width;
        }

        public static Map FromStrings(string[] strings, Sprite wallSprite)
        {
            var height = strings.Length;
            var width = strings[0].Length;
            var map = new MapCell[height, width];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    map[i, j] = GetCellByChar(strings[i][j], wallSprite);
                }
            }

            return new Map(map, height, width);
        }
        
        private static MapCell GetCellByChar(char c, Sprite sprite)
        {
            return c switch
            {
                '#' => new MapCell(MapCellType.Wall, sprite),
                _ => new MapCell(MapCellType.Empty, sprite)
            };
        }

        public bool InBound(int y, int x)
        {
            return y >= 0 && y < Height && x >= 0 && x < Width;
        }

        public MapCell At(int y, int x)
        {
            return map[y, x];
        }
    }
}