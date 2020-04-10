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

    internal class Box : IMapObject
    {
        public Box(float positionX, float positionY, Sprite sprite)
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

    internal static class EntryPoint
    {
        private static void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine25D(new EngineOptions("simple 3d game", 800, 800));
            var player = new PlayerCamera();
            var skeletonSprite = Sprite.Load("./sprites/skeleton.png");
            var wallTexture = Sprite.Load("./sprites/brick_wall.png");
            var ghostAnimation = Animation.LoadFromDirectory("./animations/ghost");
            var boxSprite = Sprite.Load("./sprites/box.png");
            var objects = new IMapObject[]
            {
                new Skeleton(5, 5, skeletonSprite),
                new Ghost(7, 7, ghostAnimation),
                new Box(18, 1.8f, boxSprite),
            };
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

            while (engine.Update(level))
                ;
        }
    }
}