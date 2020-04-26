using System;
using System.Numerics;
using level3;
using musics;
using objects.Collectables;
using objects.Environment;
using simple3d;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;
using ui;
using utils;

namespace level2
{
    public class Level2
    {
        static void Main(string[] args)
        {
            var options = new EngineOptions(
                "simple 3d game",
                720, 1280,
                true,
                UiResourcesHelper.PressStart2PFontPath,
                UiResourcesHelper.CrossSpritePath,
                UiResourcesHelper.ScrollSpritePath);
            using var engine = EngineBuilder.BuildEngine25D(options);

            StartOnEngine(engine);
        }

        public static void StartOnEngine(IEngine engine)
        {
            var loader = ResourceCachedLoader.Instance;
            var darkFloorTexture = loader.GetSprite("./sprites/dark/floor.PNG");
            var darkCeilTexture = loader.GetSprite("./sprites/dark/ceil.PNG");
            var darkWallTexture = loader.GetSprite("./sprites/dark/wall.PNG");
            var wallTexture = Sprite.Load("./sprites/wall2.png");
            var floorTexture = Sprite.Load("./sprites/floor2.png");
            var ceilingTexture = Sprite.Load("./sprites/ceiling2.png");
            var entranceTexture = Sprite.Load("./sprites/dark/entrance.PNG");
            var doorAnimation = loader.GetAnimation("./animations/door");
            var textureStorage = new MapTextureStorage(
                darkFloorTexture,
                darkCeilTexture,
                darkWallTexture,
                floorTexture,
                wallTexture,
                ceilingTexture,
                entranceTexture,
                doorAnimation);

            var scene = SceneReader.ReadFromStrings(new[]
            {
                "$$$e$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",
                "$P,,H$,,,,,,,,,,,,$AH,,,,,,,,,,,,,,,$#$e$############$",
                "$,,,A$,,,,,,,,,,,,$AH,,,,,,,,,,,S,,,$#......##..R...#$",
                "$M$$$$,,,,,,,,,,,,$,,,,,,,,,,,,,,,,,$#....S.##......#$",
                "$,$,,,,,,,,,,,,,,,$$$$$$$$$$$$,,,,,,$#...#.........L#$",
                "$,$,,,,,,,,,,,,,,,,,,,,,,$HM$$d$,,$$$#S.......M.....#$",
                "$,$$$$$$$$$$$$$$$$$$$$$$$$$,,,,$,,$###......##......#$",
                "$,,,,,,,R,,,,,,,,,,,,,R,,,,,,,,$,,$##..##...##......#$",
                "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$,,$#...##....########$",
                "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$,,$#.....M..........#$",
                "$,,,,,,,,,,,,,,S,,,,,,,,,,,,,,,,,,$#####...##########$",
                "$,,,,R,,,,,,,,,,M,,,,,,,,S,,,,,,,,$##...............#$",
                "$,,$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$.###.............#$",
                "$,,$$##################################.............#$",
                "$$,,$$$MM,,,,,,,,,,.................................#$",
                "$$$,,,$$$$,H$RS$$$AA####################d############$",
                "$$$$$,,S,$$$$$$$$$$$$...HH..AA....S.#..#.#....#######$",
                "$,,$$$$,,,,,$$$,R,,,d.....S...MM..............#######$",
                "$,,,,,$$$$,,,M,,,$$$$################.........#######$",
                "$,,,,,,,,$$$$$$$$,,,,,,,,,,,,,,,,,,,###########,,,,,,$",
                "$,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,$",
                "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",
            }, textureStorage.GetCellByChar, MathF.PI / 2);
            
            // scene.AddObject(SpiderNet.Create());
            
            scene.AddObject(SpiderNet.Create(new Vector2(2.5f, 14.5f)));
            scene.AddObject(SpiderNet.Create(new Vector2(3.5f, 14.5f)));
            scene.AddObject(SpiderNet.Create(new Vector2(1.5f, 5.5f)));
            scene.AddObject(SpiderNet.Create(new Vector2(10.5f, 7.5f)));
            scene.AddObject(SpiderNet.Create(new Vector2(27.5f, 5.5f)));
            scene.AddObject(Mushrooms.Create(new Vector2(8.5f, 7.5f)));
            scene.AddObject(Mushrooms2.Create(new Vector2(12.5f, 7.5f)));
            scene.AddObject(Mushrooms2.Create(new Vector2(33.5f, 9.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(15.5f, 7.5f)));
            scene.AddObject(Bones.Create(new Vector2(26.5f, 7.5f)));
            scene.AddObject(Bones.Create(new Vector2(26.5f, 3.5f)));
            scene.AddObject(Bones.Create(new Vector2(24.5f, 2.5f)));
            scene.AddObject(Bones.Create(new Vector2(2.5f, 10.5f)));
            scene.AddObject(Bones.Create(new Vector2(40.5f, 9.5f)));
            scene.AddObject(SpiderNet.Create(new Vector2(12.5f, 11.5f)));
            scene.AddObject(Bones.Create(new Vector2(16.5f, 10.5f)));
            scene.AddObject(Mushrooms.Create(new Vector2(22.5f, 1.5f)));
            scene.AddObject(SpiderNet.Create(new Vector2(35.5f, 1.5f)));

            scene.AddObject(
                Note.Create(new Vector2(37.5f - 18, 3.5f),
                "Ещё одна запись в дневнике, я становлюсь одержимым?\n" +
                "Этот ход через официальную лабораторию\n" +
                "был неплохо спрятан от внешних глаз, но я осторожный\n" +
                "Пожалуй, поставлю сюда какую-нибудь охрану.\n" +
                "Желательно чтобы она не могла разболтать, что она охраняет."));

            scene.AddObject(
                Note.Create(new Vector2(40.5f, 18.5f), 
                    "По моим последним данным - источник магии на планете - Солнце\n" +
                    "Многие бы отказались от своей идеи, имея такое препятствие.\n" +
                    "Многие, но не я.\n" +
                    "Мне повезло родиться могущественным волшебником. Даже очень.\n" +
                    "Одно из того, на что могут повлиять волшебники - гравитация.\n" +
                    "Через 2 года недалеко от нашей системы будет пролетать чёрная дыра\n" +
                    "Такой шанс выпадает раз в тысячу лет. Не воспользоваться - грех."));

            scene.AddObject(
                Note.Create(new Vector2(50.5f, 11.5f),
                    "Да, все расчёты верны!\n" +
                    "Небольшой манёвр поможет планете вырваться от нашего Солнца!\n" +
                    "Притянуты чёрной дырой мы не будем, а просто улетим с текущей орбиты\n" +
                    "Однако проблема - полёт займёт несколько десятков тысяч лет\n" +
                    "За столько лет вся разумная жизнь на планете вымрет\n" +
                    " \n" +
                    "Но и на это у меня есть ответ: заморозим время на планете в некоторой\n" +
                    "области, где укроются люди\n" +
                    "Главное, чтобы никто не мешал - никому не понравится такой полёт\n"));


            scene.AddObject(Lamp1.Create(new Vector2(36.5f, 17.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(40.5f, 16.5f)));

            var music = loader.GetMusic(MusicResourceHelper.EnvironmentDungeonMusic);
            music.Play(-1);

            Trigger.AddTrigger(3, 1, (scene) =>
            {
                scene.Player.CurrentMonologue = new Monologue(new[]
                {
                    ("Пахнет крысами. Опять.", 3000)
                });
            }, false);

            Trigger.AddTrigger(7, 22, (scene) =>
            {
                scene.Player.CurrentMonologue = new Monologue(new[]
                {
                    ("Кости? Кажется, тут есть что-то опаснее крыс", 3000)
                });
            }, false);
            
            scene.Map.At(5, 30).SetTag("door1");
            Trigger.AddTrigger(6, 30, (scene) =>
            {
                Map.GetCellByTag("door1").StartAnimatiom(() => { Map.GetCellByTag("door1").Type = MapCellType.Empty; });
            });
            
            scene.Map.At(17, 20).SetTag("door2");
            Trigger.AddTrigger(17, 19, scene =>
                {
                    scene.Player.CurrentMonologue = new Monologue(new[]
                    {
                        ("Кажется, это вход в какую-то лабораторию", 3000),
                        ("Главное, что-бы её хозяин не обиделся на меня за меня \n" +
                         "за то, что я вломился без разрешения", 4000)
                    });

                    Map.GetCellByTag("door2").StartAnimatiom(() =>
                        {
                            Map.GetCellByTag("door2").Type = MapCellType.Empty;
                        });
                });
            
            scene.Map.At(15, 40).SetTag("door3");
            Trigger.AddTrigger(17, 40, scene =>
            {
                scene.Player.CurrentMonologue = new Monologue(new[]
                {
                    ("От двери веет могильным холодом...", 2000)
                });

                Map.GetCellByTag("door3").StartAnimatiom(() =>
                {
                    Map.GetCellByTag("door3").Type = MapCellType.Empty;
                });
            });
            
            Trigger.AddTrigger(2, 39, (scene) =>
            {
                Map.ClearTags();
                Level3.StartOnEngine(engine);
            }, false);


            while (engine.Update(scene))
            {
                if (scene.Player.Health <= 0)
                {
                    Map.ClearTags();
                    StartOnEngine(engine);
                }
            }

            MusicHelper.StopPlaying();
        }
        
        private class MapTextureStorage
        {
            private readonly Sprite ceilingTexture;
            private readonly Sprite wallTexture;
            private readonly Sprite floorTexture;
            private readonly Sprite darkWallTexture;
            private readonly Sprite darkCeilTexture;
            private readonly Sprite darkFloorTexture;
            private readonly Sprite entranceTexture;
            private readonly Animation doorAnimation;

            public MapTextureStorage(Sprite darkFloorTexture, Sprite darkCeilTexture, Sprite darkWallTexture, Sprite floorTexture, Sprite wallTexture, Sprite ceilingTexture, Sprite entranceTexture, Animation doorAnimation)
            {
                this.darkFloorTexture = darkFloorTexture;
                this.darkCeilTexture = darkCeilTexture;
                this.darkWallTexture = darkWallTexture;
                this.floorTexture = floorTexture;
                this.wallTexture = wallTexture;
                this.ceilingTexture = ceilingTexture;
                this.entranceTexture = entranceTexture;
                this.doorAnimation = doorAnimation;
            }

            public MapCell GetCellByChar(char c)
            {
                switch (c)
                {
                    case '.':
                        return new MapCell(MapCellType.Empty, wallTexture, floorTexture, ceilingTexture);
                    case '#':
                        return new MapCell(MapCellType.Wall, wallTexture, floorTexture, ceilingTexture);
                    case '$':
                        return new MapCell(MapCellType.Wall, darkWallTexture, darkCeilTexture, darkCeilTexture);
                    case 'e':
                        return new MapCell(MapCellType.Wall, entranceTexture, darkCeilTexture, darkCeilTexture);
                    case 'd':
                        return new MapCell(MapCellType.Door, doorAnimation, darkFloorTexture, darkCeilTexture);
                    case ',':
                    case 'P':
                        return new MapCell(MapCellType.Empty, darkWallTexture, darkFloorTexture, darkCeilTexture);
                    default:
                        return new MapCell(MapCellType.Empty, darkWallTexture, darkFloorTexture, darkCeilTexture);
                }
            }
        }
    }
}