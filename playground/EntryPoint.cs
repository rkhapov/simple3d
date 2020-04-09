using simple3d.Builder;
using simple3d.Scene;

namespace playground
{
    internal class Skeleton : IMapObject
    {
        public Skeleton(float positionX, float positionY, Sprite sprite)
        {
            PositionX = positionX;
            PositionY = positionY;
            Sprite = sprite;
        }

        public float PositionX { get; }
        public float PositionY { get; }
        public Sprite Sprite { get; }
    }

    internal static class EntryPoint
    {
        private static void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine(new EngineOptions("simple 3d game", 800, 600));
            var player = new PlayerCamera();
            var skeletonSprite = Sprite.Load("./sprites/skeleton.png");
            var wallTexture = Sprite.Load("./sprites/brick_wall.png");
            var skeletons = new[] { new Skeleton(5, 5, skeletonSprite) };
            var map = Map.FromStrings(new[]
            {
                "###############################",
                "#.........#...................#",
                "#..#..........#...............#",
                "#.........############........#",
                "#.........#..........#........#",
                "#.........#..........###......#",
                "#.........#..........#........#",
                "####......##########.#........#",
                "##...................#......###",
                "#........####........#........#",
                "#........#..#........#........#",
                "#........#..#........###......#",
                "#####.####..####.....#........#",
                "#####.####..####.....#........#",
                "#....................#......###",
                "#....................#........#",
                "#####.####..####.....###......#",
                "#####.####..####.....#........#",
                "#....................#........#",
                "###############################"
            }, wallTexture);
            var level = new Level(player, map, skeletons);

            engine.RunLevel(level);
        }
    }
}