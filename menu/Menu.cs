using System;
using System.Numerics;
using System.Threading.Channels;
using musics;
using objects;
using objects.Collectables;
using objects.Environment;
using objects.Monsters;
using objects.Monsters.Algorithms;
using objects.Weapons;
using simple3d;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using ui;
using utils;

namespace menu
{
    internal class InvisibleWall : BaseStaticMapObject
    {
        public InvisibleWall(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle, new Sprite(new [] {1}, 1,1))
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
                    true,
                    UiResourcesHelper.PressStart2PFontPath,
                    UiResourcesHelper.CrossSpritePath,
                    UiResourcesHelper.ScrollSpritePath));
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            
            var startButtonTexture = Sprite.Load("./sprites/startbutton_v4.png");
            var exitButton = Sprite.Load("./sprites/exitbutton_v1.png");
            var scoreboard = Sprite.Load("./sprites/scoreboard.png");

            var directionText = Sprite.Load("./sprites/direction.png");
            var controlsText = Sprite.Load("./sprites/controls_v5.png");
            var statusBarInfo = Sprite.Load("./sprites/statusbarinfo.png");
            var collectiblesText = Sprite.Load("./sprites/collectibles.png");
            var shootingTutorial = Sprite.Load("./sprites/shooting_tutorial.png");
            var fightingTutorial = Sprite.Load("./sprites/fight_tutorial.png");
            var magicTutorial = Sprite.Load("./sprites/magic_tutorial.png");
            
            var doorAnimation = ResourceCachedLoader.Instance.GetAnimation("./animations/door");
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, controlsText, 
                startButtonTexture, exitButton, scoreboard, statusBarInfo, shootingTutorial, doorAnimation,
                directionText, collectiblesText, fightingTutorial, magicTutorial);

            var scene = SceneReader.ReadFromStrings(
                new[]
                {
                    "###############################",
                    "#..........######.P.#.........#",
                    "#..........#....d...d.........r",
                    "#..........c....#...#.........#",
                    "#..#########....##p##.........#",
                    "#..#.A.M.H......#...#.........#",
                    "#..l.....A......#...#.........#",
                    "#..##d######i#n##...#.........s",
                    "#..#....#...........#.........#",
                    "#..#....#...........#.........#",
                    "#..f.R..#...........#.........#",
                    "#..##d###############.........#",
                    "#..#....#.....H..A.#..........#",
                    "#..#....d.......S..d..........e",
                    "#..#.S..#.....H....#..........#",
                    "#####m#########################"
                }, storage.GetCellByChar, 0);
            
            scene.AddObject(Note.Create(new Vector2(7.0f, 6.0f), "Просто записка.\nЧтобы закрыть нажмите ESC"));

            var map = scene.Map;
            
            map.At(2, 20).SetTag("tutorialDoor");
            Trigger.AddTrigger(new Vector2(19f, 2f), 
                (scene) => { Map.GetCellByTag("tutorialDoor").StartAnimatiom(() => { Map.GetCellByTag("tutorialDoor").Type = MapCellType.Empty; }); }, false);

            map.At(2, 16).SetTag("menuDoor");
            Trigger.AddTrigger(new Vector2(17f, 2f),
                (scene) => Map.GetCellByTag("menuDoor")
                    .StartAnimatiom(() => Map.GetCellByTag("menuDoor").Type = MapCellType.Empty), false);
            
            map.At(7,5).SetTag("shootingDoor");
            Trigger.AddTrigger(new Vector2(5f, 6f),
                scene => Map.GetCellByTag("shootingDoor")
                    .StartAnimatiom(() => Map.GetCellByTag("shootingDoor").Type = MapCellType.Empty));
            
            map.At(11, 5).SetTag("fightingDoor");
            Trigger.AddTrigger(new Vector2(5f, 10f),
                scene => Map.GetCellByTag("fightingDoor")
                    .StartAnimatiom(() => Map.GetCellByTag("fightingDoor").Type = MapCellType.Empty));
            
            map.At(13,8).SetTag("magicDoor");
            Trigger.AddTrigger(new Vector2(7f, 13f),
                scene => Map.GetCellByTag("magicDoor")
                    .StartAnimatiom(() => Map.GetCellByTag("magicDoor").Type = MapCellType.Empty));
            
            map.At(13, 19).SetTag("tutorialExit");
            Trigger.AddTrigger(new Vector2(18f, 13f),
                scene => Map.GetCellByTag("tutorialExit")
                    .StartAnimatiom(() => Map.GetCellByTag("tutorialExit").Type = MapCellType.Empty));

            var backGroundMusic = ResourceCachedLoader.Instance.GetMusic(MusicResourceHelper.EnvironmentDungeonMusic);
            backGroundMusic.Play(-1);

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
            private readonly Sprite shootingTutorial;
            private readonly Animation doorAnimation;
            private readonly Sprite directionText;
            private readonly Sprite collectiblesText;
            private readonly Sprite fightingTutorial;
            private readonly Sprite magicTutorial;

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite controls,
                Sprite startButton, Sprite exitButton, Sprite scoreboard, Sprite statusBarInfo, Sprite shootingTutorial, 
                Animation doorAnimation, Sprite directionText, Sprite collectiblesText, Sprite fightingTutorial, Sprite magicTutorial)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.controls = controls;
                this.startButton = startButton;
                this.exitButton = exitButton;
                this.scoreboard = scoreboard;
                this.statusBarInfo = statusBarInfo;
                this.shootingTutorial = shootingTutorial;
                this.doorAnimation = doorAnimation;
                this.directionText = directionText;
                this.collectiblesText = collectiblesText;
                this.fightingTutorial = fightingTutorial;
                this.magicTutorial = magicTutorial;
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
                    'l' => new MapCell(MapCellType.Wall, shootingTutorial, shootingTutorial, ceilingTexture),
                    'd' => new MapCell(MapCellType.Door, doorAnimation, wallTexture, ceilingTexture),
                    'p' => new MapCell(MapCellType.Wall, directionText, directionText, ceilingTexture),
                    'n' => new MapCell(MapCellType.Wall, collectiblesText, collectiblesText, ceilingTexture),
                    'f' => new MapCell(MapCellType.Wall, fightingTutorial, fightingTutorial, ceilingTexture),
                    'm' => new MapCell(MapCellType.Wall, magicTutorial, magicTutorial, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }
        }
    }
}