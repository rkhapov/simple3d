using System;
using System.Numerics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

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
        public void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnShoot(Scene scene, ShootingWeapon weapon)
        {
        }
    }

    internal abstract class BaseAnimatedStaticMapObject : IMapObject
    {
        private readonly Animation animation;

        protected BaseAnimatedStaticMapObject(Vector2 position, Vector2 size, float directionAngle, Animation animation)
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

        public void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public abstract void OnShoot(Scene scene, ShootingWeapon weapon);
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

    internal class Ghost : BaseAnimatedStaticMapObject
    {
        public Ghost(Vector2 position, Vector2 size, float directionAngle, Animation animation) : base(position, size, directionAngle, animation)
        {
        }

        public override void OnShoot(Scene scene, ShootingWeapon weapon)
        {
            Console.WriteLine($"{this} has been hit");
            scene.RemoveObject(this);
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
                Endurance += elapsedMilliseconds * 0.005f;
                Endurance = MathF.Min(Endurance, MaxEndurance);
            }
            
            Weapon.UpdateAnimation(elapsedMilliseconds);
        }

        public override void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public override void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public override void OnShoot(Scene scene, ShootingWeapon weapon)
        {
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

    internal class Sword : MeleeWeapon
    {
        public Sword(Animation staticAnimation, Animation attackLeftAnimation, Animation attackRightAnimation,
            Animation blockLeftAnimation, Animation blockRightAnimation, Animation movingAnimation) : base(
            staticAnimation, attackLeftAnimation, attackRightAnimation, blockLeftAnimation, blockRightAnimation,
            movingAnimation)
        {
        }
    }

    internal class Arrow : IMapObject
    {
        public Arrow(Vector2 position, Sprite sprite, float directionAngle)
        {
            Position = position;
            Sprite = sprite;
            DirectionAngle = directionAngle;
        }

        public Vector2 Position { get; private set; }
        public Vector2 Size { get; } = Vector2.One;
        public float DirectionAngle { get; }
        public Sprite Sprite { get; }

        public void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            var xRayUnit = MathF.Sin(DirectionAngle);
            var yRayUnit = MathF.Cos(DirectionAngle);
            var dx = xRayUnit * 0.05f * elapsedMilliseconds;
            var dy = yRayUnit * 0.05f * elapsedMilliseconds;
            var newPosition = Position + new Vector2(dx, dy);
            var testX = (int) newPosition.X;
            var testY = (int) newPosition.Y;

            if (!scene.Map.InBound(testY, testX) || scene.Map.At(testY, testX).Type == MapCellType.Wall)
            {
                Console.WriteLine($"hit wall at {testY} {testX}");
                scene.RemoveObject(this);
                return;
            }

            var vertices = this.GetRotatedVertices();
            foreach (var obj in scene.Objects)
            {
                if (obj is Arrow)
                    continue;

                if (GeometryHelper.IsRectanglesIntersects(vertices, obj.GetRotatedVertices()))
                {
                    Console.WriteLine($"hit {obj} at {newPosition}");
                    obj.OnShoot(scene, null);
                    scene.RemoveObject(this);
                    return;
                }
            }

            Position = newPosition;
        }

        public void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnShoot(Scene scene, ShootingWeapon weapon)
        {
        }
    }

    internal class Bow : ShootingWeapon
    {
        private readonly Sprite arrowSprite;
        
        public Bow(
            Animation staticAnimation,
            Animation movingAnimation,
            Animation shootingAnimation,
            Sprite arrowSprite) : base(staticAnimation, movingAnimation, shootingAnimation)
        {
            this.arrowSprite = arrowSprite;
        }

        public override void MakeShoot(Scene scene)
        {
            scene.AddObject(new Arrow(scene.Player.Position, arrowSprite, scene.Player.DirectionAngle));
        }
    }

    internal static class EntryPoint
    {
        private static unsafe void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine25D(new EngineOptions("simple 3d game", 720, 1280, true));
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2);
            var skeletonSprite = Sprite.Load("./sprites/skeleton.png");
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var windowTexture = Sprite.Load("./sprites/window.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var greenLightTexture = Sprite.Load("./sprites/greenlight.png");
            var ghostAnimation = Animation.LoadFromDirectory("./animations/ghost");
            var sword = new Sword(
                Animation.LoadFromDirectory("./animations/sword_static"),
                Animation.LoadFromDirectory("./animations/sword_static"),
                Animation.LoadFromDirectory("./animations/sword_static"),
                Animation.LoadFromDirectory("./animations/sword_left_block"),
                Animation.LoadFromDirectory("./animations/sword_right_block"),
                Animation.LoadFromDirectory("./animations/sword_static"));
            var bow = new Bow(
                Animation.LoadFromDirectory("./animations/bow_static"),
                Animation.LoadFromDirectory("./animations/bow_moving"),
                Animation.LoadFromDirectory("./animations/bow_shoot"),
                Sprite.Load("./sprites/arrow.png"));
            player.Weapon = bow;
            var objects = new IMapObject[]
            {
                new Ghost(new Vector2(7.0f, 7.0f), new Vector2(0.5f, 0.5f), 0.0f, ghostAnimation),
                new GreenLight(new Vector2(8.0f, 8.0f), new Vector2(0, 0), 0, greenLightTexture),
            };
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, windowTexture);
            var map = Map.FromStrings(new[]
            {
                "###############################",
                "#.........#...................#",
                "#..#..........#...............#",
                "#.........############........#",
                "#.........o..........#........#",
                "#.........o..........###......#",
                "#.....o...o..........#........#",
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
            }, storage.GetCellByChar);
            var level = new Scene(player, map, objects);
            
            while (engine.Update(level))
            {
            }
        }

        private class MapTextureStorage
        {
            private readonly Sprite ceilingTexture;
            private readonly Sprite wallTexture;
            private readonly Sprite floorTexture;
            private readonly Sprite windowTexture;

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite windowTexture)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.windowTexture = windowTexture;
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    '#' => new MapCell(MapCellType.Wall, wallTexture, ceilingTexture),
                    'o' => new MapCell(MapCellType.Window, windowTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, ceilingTexture)
                };
            }
        }
    }
}