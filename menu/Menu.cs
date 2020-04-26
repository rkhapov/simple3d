using System;
using System.Numerics;
using objects;
using objects.Environment;
using objects.Monsters;
using objects.Weapons;
using simple3d;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using ui;

namespace menu
{
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
            using var engine = EngineBuilder.BuildEngine25D(
                new EngineOptions("simple 3d game", 720, 1280,
                    false,
                    UiResourcesHelper.PressStart2PFontPath,
                    UiResourcesHelper.CrossSpritePath,
                    UiResourcesHelper.ScrollSpritePath));
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var startButtonTexture = Sprite.Load("./sprites/startbutton_v4.png");
            var exitButton = Sprite.Load("./sprites/exitbutton_v1.png");
            var controlsText = Sprite.Load("./sprites/controls_v4.png");
            var scoreboard = Sprite.Load("./sprites/scoreboard.png");
            var statusBarInfo = Sprite.Load("./sprites/statusbarinfo.png");
            var tutorialEnd = Sprite.Load("./sprites/tutorialend.png");

            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, controlsText, 
                startButtonTexture, exitButton, scoreboard, statusBarInfo, tutorialEnd);
            var scene = SceneReader.ReadFromStrings(
                new[]
                {
                    "##c###########",
                    "#....#l###...#",
                    "#..#.#.......e",
                    "#..#.#.#.....#",
                    "#..#..S#.....s",
                    "#..#i##......#",
                    "#..#.........r",
                    "#.P#.........#",
                    "##############"
                }, storage.GetCellByChar, MathF.PI);
            
            scene.AddObject(new Invisible(new Vector2(9.0f, 3.0f), new Vector2(0.1f, 10.0f), 0));
            
            while (engine.Update(scene))
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