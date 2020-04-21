using System;
using System.Numerics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;

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
    }

    // internal abstract class BaseAnimatedMapObject : IMapObject
    // {
    //     private readonly Animation animation;
    //
    //     protected BaseAnimatedMapObject(Vector2 position, Vector2 size, float directionAngle, Animation animation)
    //     {
    //         Position = position;
    //         Size = size;
    //         DirectionAngle = directionAngle;
    //         this.animation = animation;
    //     }
    //
    //     public Vector2 Position { get; }
    //     public Vector2 Size { get; }
    //     public float DirectionAngle { get; }
    //     public Sprite Sprite => animation.CurrentFrame;
    //
    //     public virtual void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
    //     {
    //         animation.UpdateFrame(elapsedMilliseconds);
    //     }
    // }

    internal class MyPlayer : Player
    {
        public MyPlayer(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
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

    internal class StartButton : BaseStaticMapObject
    {
        public StartButton(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }
        
        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
        }
    }
    internal class ExitButton : BaseStaticMapObject
    {
        public ExitButton(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }
        
        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
        }
    }

    internal class Controlls : BaseStaticMapObject
    {
        public Controlls(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            // throw new NotImplementedException();
        }
    } internal class Scoreboard : BaseStaticMapObject
    {
        public Scoreboard(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            // throw new NotImplementedException();
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
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2);
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var greenLightTexture = Sprite.Load("./sprites/greenlight.png");
            var startButtonTexture = Sprite.Load("./sprites/startbutton_v4.png");
            var exitButton = Sprite.Load("./sprites/exitbutton_v1.png");
            var controlsText = Sprite.Load("./sprites/controls_v4.png");
            var scoreboard = Sprite.Load("./sprites/scoreboard.png");
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, controlsText, startButtonTexture, exitButton);
            var objects = new IMapObject[]
            {
                new GreenLight(new Vector2(2.0f, 4.0f), new Vector2(0, 0), 0, greenLightTexture),
                new GreenLight(new Vector2(6.0f, 4.0f), new Vector2(0, 0), 0, greenLightTexture),
                new Invisible(new Vector2(8.0f, 4.0f), new Vector2(0.1f, 10.0f), 0), 
            };
            var map = Map.FromStrings(new[]
            {
                "#####c####",
                "#........#",
                "#........#",
                "#........#",
                "c........s",
                "#........#",
                "#........e",
                "#........#",
                "#####c####"
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

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite controls, Sprite startButton, Sprite exitButton)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.controls = controls;
                this.startButton = startButton;
                this.exitButton = exitButton;
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    '#' => new MapCell(MapCellType.Wall, wallTexture, ceilingTexture),
                    'c' => new MapCell(MapCellType.Wall, controls, ceilingTexture),
                    's' => new MapCell(MapCellType.Wall, startButton, ceilingTexture),
                    'e' => new MapCell(MapCellType.Wall, exitButton, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, ceilingTexture)
                };
            }
        }
    }
}