using System;
using System.Numerics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;

namespace playground
{
    internal abstract class BaseStaticMapObject : IMapObject
    {
        protected BaseStaticMapObject(Vector2 position, Vector2 size, float directionAngle, Sprite sprite)
        {
            Position = position;
            Sprite = sprite;
            Size = size;
            DirectionAngle = directionAngle;
        }

        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public float DirectionAngle { get; }
        public Sprite Sprite { get; }

        public abstract void OnWorldUpdate(Scene scene, float elapsedMilliseconds);
    }

    internal abstract class BaseAnimatedMapObject : IMapObject
    {
        private readonly Animation animation;

        protected BaseAnimatedMapObject(Vector2 position, Vector2 size, float directionAngle, Animation animation)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
            this.animation = animation;
        }

        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public float DirectionAngle { get; }
        public Sprite Sprite => animation.CurrentFrame;

        public virtual void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            animation.UpdateFrame(elapsedMilliseconds);
        }
    }
    
    internal class Skeleton : BaseStaticMapObject
    {
        public Skeleton(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
        }
    }

    internal class Ghost : BaseAnimatedMapObject
    {
        public Ghost(Vector2 position, Vector2 size, float directionAngle, Animation animation) : base(position, size, directionAngle, animation)
        {
        }
    }

    internal class MyPlayer : Player
    {
        public MyPlayer(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            if (Endurance < MaxEndurance)
            {
                Endurance += elapsedMilliseconds * 0.001f;
                Endurance = MathF.Min(Endurance, MaxEndurance);
            }
        }
    }

    internal class GreenLight : BaseStaticMapObject
    {
        public GreenLight(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
        }
    }

    internal static class EntryPoint
    {
        private static unsafe void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine25D(new EngineOptions("simple 3d game", 500, 500, true));
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2);
            var skeletonSprite = Sprite.Load("./sprites/skeleton.png");
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var greenLightTexture = Sprite.Load("./sprites/greenlight.png");
            var ghostAnimation = Animation.LoadFromDirectory("./animations/ghost");
            var objects = new IMapObject[]
            {
                new Ghost(new Vector2(7.0f, 7.0f), new Vector2(0.5f, 0.5f), 0.0f, ghostAnimation),
                new GreenLight(new Vector2(8.0f, 8.0f), new Vector2(0, 0), 0, greenLightTexture),
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
            }, wallTexture, floorTexture, ceilingTexture);
            var level = new Scene(player, map, objects);
            
            while (engine.Update(level))
            {
            }
        }
    }
}