using System;
using System.Numerics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace menu
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
                if (obj == this)
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

    internal class Invisible : BaseStaticMapObject
    {
        public Invisible(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle, new Sprite(new [] {1}, 1,1))
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            // throw new NotImplementedException();
        }
    }

    internal static class Menu
    {
        private static void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine25D(new EngineOptions("simple 3d game", 720, 1280, false));
            var player = new MyPlayer(new Vector2(2.0f, 7.0f), new Vector2(0.3f, 0.3f), MathF.PI);
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var greenLightTexture = Sprite.Load("./sprites/greenlight.png");
            var startButtonTexture = Sprite.Load("./sprites/startbutton_v4.png");
            var exitButton = Sprite.Load("./sprites/exitbutton_v1.png");
            var controlsText = Sprite.Load("./sprites/controls_v4.png");
            var scoreboard = Sprite.Load("./sprites/scoreboard.png");
            var statusBarInfo = Sprite.Load("./sprites/statusbarinfo.png");
            var tutorialEnd = Sprite.Load("./sprites/tutorialend.png");
            var ghost = Animation.LoadFromDirectory("./animations/ghost");
            
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
            
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, controlsText, 
                startButtonTexture, exitButton, scoreboard, statusBarInfo, tutorialEnd);
            var objects = new IMapObject[]
            {
                new GreenLight(new Vector2(2.0f, 4.0f), new Vector2(0, 0), 0, greenLightTexture),
                new GreenLight(new Vector2(6.0f, 4.0f), new Vector2(0, 0), 0, greenLightTexture),
                new Invisible(new Vector2(9.0f, 3.0f), new Vector2(0.1f, 10.0f), 0), 
                new Ghost(new Vector2(8.5f, 2.5f), new Vector2(1.0f, 1.0f), 0,  ghost) 
            };
            var map = Map.FromStrings(new[]
            {
                "##c###########",
                "#....#l###...#",
                "#..#.#.......e",
                "#..#.#.#.....#",
                "#..#...#.....s",
                "#..#i##......#",
                "#..#.........r",
                "#..#.........#",
                "##############"
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
            private readonly Sprite controls;
            private readonly Sprite startButton;
            private readonly Sprite exitButton;
            private readonly Sprite scoreboard;
            private readonly Sprite statusBarInfo;
            private readonly Sprite tutorialEnd;

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite controls,
                Sprite startButton, Sprite exitButton, Sprite scoreboard, Sprite statusBarInfo, Sprite tutorialEnd)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.controls = controls;
                this.startButton = startButton;
                this.exitButton = exitButton;
                this.scoreboard = scoreboard;
                this.statusBarInfo = statusBarInfo;
                this.tutorialEnd = tutorialEnd;
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    '#' => new MapCell(MapCellType.Wall, wallTexture, wallTexture, ceilingTexture),
                    'c' => new MapCell(MapCellType.Wall, controls, controls, ceilingTexture),
                    's' => new MapCell(MapCellType.Wall, startButton, startButton, ceilingTexture),
                    'e' => new MapCell(MapCellType.Wall, exitButton, exitButton, ceilingTexture),
                    'r' => new MapCell(MapCellType.Wall, scoreboard, scoreboard, ceilingTexture),
                    'i' => new MapCell(MapCellType.Wall, statusBarInfo, statusBarInfo, ceilingTexture),
                    'l' => new MapCell(MapCellType.Wall, tutorialEnd, tutorialEnd, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }
        }
    }
}