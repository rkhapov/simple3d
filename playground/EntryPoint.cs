using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;

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

        public void OnWorldUpdate(Level level, float elapsedMilliseconds)
        {
            //nothing
        }
    }

    internal class Ghost : IMapObject
    {
        private readonly Animation animation;

        public Ghost(float positionX, float positionY, Animation animation)
        {
            this.animation = animation;
            PositionX = positionX;
            PositionY = positionY;
        }

        public float PositionX { get; }
        public float PositionY { get; }
        public Sprite Sprite => animation.CurrentFrame;

        public void OnWorldUpdate(Level level, float elapsedMilliseconds)
        {
            animation.UpdateFrame(elapsedMilliseconds);
        }
    }

    internal static class EntryPoint
    {
        private static void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine(new EngineOptions("simple 3d game", 800, 600));
            var player = new PlayerCamera();
            var skeletonSprite = Sprite.Load("./sprites/skeleton.png");
            var wallTexture = Sprite.Load("./sprites/brick_wall.png");
            var ghostAnimation = Animation.LoadFromDirectory("./animations/ghost");
            var objects = new[] { new Skeleton(5, 5, skeletonSprite), (IMapObject) new Ghost(7, 7, ghostAnimation) };
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
            var level = new Level(player, map, objects);

            engine.RunLevel(level);
        }
    }
}