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
                    "###########################################",
                    "#P.###....................................#",
                    "#.####....................................#",
                    "#d##......................................#",
                    "#.##......................................m",
                    "#.##......................................f",
                    "#.##......................................l",
                    "#.####################....................n",
                    "#....H.....A..............................i",
                    "#######################...................c",
                    "#.#.......................................p",
                    "#.#..............L........................#",
                    "#.........................................#",
                    "#.........................................#",
                    "#.........................................#",
                    "###########################################"
                }, storage.GetCellByChar, MathF.PI / 2);
            
            scene.AddObject(new InvisibleWall(new Vector2(9.0f, 3.0f), new Vector2(0.1f, 10.0f), 0));
            scene.AddObject(Note.Create(new Vector2(15.5f, 8.5f), "Просто записка.\nЧтобы закрыть нажмите ESC"));
            
            var map = scene.Map;
            map.At(3, 1).SetTag("startDoor");
            Trigger.AddTrigger(new Vector2(1f, 1f), 
                (scene) => { Map.GetCellByTag("startDoor").StartAnimatiom(() => { Map.GetCellByTag("startDoor").Type = MapCellType.Empty; }); });

            Trigger.AddTrigger(new Vector2(1f, 1f), scene =>
            {
                scene.Player.CurrentMonologue = new Monologue(
                    new[]
                    {
                        ("Движение: WASD", 2500),
                        ("Камера: Стрелочки", 2500),
                        ("Взаимодействие: E\nОткройте дверь и выберитесь наружу!", 2500),
                    });
            }, false);

            Trigger.AddTrigger(3, 1, scene =>
            {
                scene.Player.CurrentMonologue = new Monologue(
                    new[]
                    {
                        ("Хорошо", 1500),
                        ("А это предметы, которые можно собирать", 1500),
                    });
            }, false);

            Trigger.AddTrigger(8, 2, scene =>
            {
                scene.Player.CurrentMonologue = new Monologue(
                    new[]
                    {
                        ("Это зелье восстанавливает здоровье", 1500),
                    });
            }, false);
            
            Trigger.AddTrigger(8, 6, scene =>
            {
                scene.Player.CurrentMonologue = new Monologue(
                    new[]
                    {
                        ("А это стрелы, лишними не будут", 1500),
                    });
            }, false);

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
                    'd' => new MapCell(MapCellType.TransparentObj, doorAnimation, wallTexture, ceilingTexture),
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