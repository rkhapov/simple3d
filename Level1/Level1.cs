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
namespace Level1
{
    public static class Level1
    {
        public class StaticLich : BaseStaticMapObject
        {
            public StaticLich(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
            {

            }

            public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
            {

            }
        }
        private static unsafe void Main(string[] args)
        {
            var options = new EngineOptions(
                "simple 3d game",
                720, 1280,
                false,
                UiResourcesHelper.PressStart2PFontPath,
                UiResourcesHelper.CrossSpritePath,
                UiResourcesHelper.ScrollSpritePath);
            using var engine = EngineBuilder.BuildEngine25D(options);
            StartOnEngine(engine);
        }

        public static void StartOnEngine(IEngine engine)
        {
            var resourceLoader = ResourceCachedLoader.Instance;
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2, 0);
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var windowTexture = Sprite.Load("./sprites/window.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var bedTexture = Sprite.Load("./sprites/bed.png");
            var lichSprite = Sprite.Load("./animations/lich/static/1.png");
            var sword = Sword.Create(resourceLoader);
            var bow = Bow.Create(resourceLoader);
            var doorAnimation = resourceLoader.GetAnimation("./animations/door");
            player.Weapons = new Weapon[] { sword, bow };
            var backGroundMusic = resourceLoader.GetMusic(MusicResourceHelper.EnvironmentDungeonMusic);

            backGroundMusic.Play(-1);
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, windowTexture, bedTexture, doorAnimation);
            var scene = SceneReader.ReadFromStrings(
                new[]
                {
                "#################################################@@@@@@@",
                "#....#...#.........................#............#@.....@",
                "#....#...#.........................#............#@.....@",
                "#.1..#...#a........................#.........9..#@.....@",
                "##d#######d##......................#.....####d######d###",
                "#......#AHl@#......................#.....#..........e..#",
                "#......oAHA@#......................#.....#.............#",
                "#......#AHA@#......................#.....#.............#",
                "#......#o###.................%######.....#.............#",
                "#......#...#.................d8..........#.............#",
                "#..........#.................%######.....#.............#",
                "#......#############...............#.....#.............#",
                "#.2....#.HHH....AAA#...............#.....#...........c.#",
                "##d###########d#######################.##############d##",
                "#........AH#R.5.....R.#....#######.............#.......#",
                "#........HA#.....o....#.......%................#.......#",
                "#...@@.....#.....o....#....#...................o.R.#####",
                "#....@@....#.....o...7dP...#...................#.....R.#",
                "#.........3d.....o....#....o...................#.....f.#",
                "#.R........#....4.....#...R##########################d##",
                "############o###d###########@..........................#",
                "#....................R#AAHH########..###################",
                "#..%.......R..........#....%...........#...............#",
                "#.......$........%R...#................#......######...#",
                "#.....................$................#..#.....#R.#...#",
                "$R.......$.......oH..@#................#..#.....#..#...#",
                "######...............@#................#...........#RgR#",
                "@....#......$.........#$$$$$$$$........#....#########d##",
                "@.HA.#......R........#$.......$........#....#R...#.HH..@",
                "@@HA.d6..............#$.......$........##d###....#..HH.@",
                "@.HA.#....@@@........#$E.....dl..........i.......oAAHHt@",
                "@@@@@###$##%###$##%###$$$$$$$$$$##################@@@@@@",
                }, storage.GetCellByChar, MathF.PI / 2);

            scene.Map.At(4, 2).SetTag("startDoor");
            Trigger.AddTrigger(new Vector2(2f, 3f), (scene) =>
            {
                Console.WriteLine("OPPEEEN THE DOOOOR");
                scene.Player.CurrentMonologue = new Monologue(
                    new[] {
                        ("Хмм... открыто", 2000)
                    }
                    );
                Map.GetCellByTag("startDoor").StartAnimatiom(() => { Map.GetCellByTag("startDoor").Type = MapCellType.Empty; });
            });
            Trigger.AddTrigger(new Vector2(6f, 6f), (scene) =>
            {
                scene.Player.CurrentMonologue = new Monologue(
                    new[] {
                        ("Сколько всего... интересно кто это все сюда принес и зачем...", 3000)
                    }
                    );
            
         }, false);
            scene.AddObject(Note.Create(new Vector2(2,   8), "Этот мир так чудесен, бузупречная флора и фауна,\n безупречный баланс рас... \n "));
           
            CreateTextTrigger(new Vector2(2f, 5f), new[] { ("Интересно, кто оставил эту записку?", 3000) }); 
            scene.AddObject(Note.Create(new Vector2(2, 8), "Этот мир так чудесен, бузупречная флора и фауна,\n безупречный баланс рас... \n "));
            scene.AddObject(Note.Create(new Vector2(9, 9), "Люди - средние по физическим данным, но самые умные представители\n разумных существ." +
                "Особых конфронтаций ни с кем нет,\n но ксенофобии к другим расам, конечно, достаточно."));
            CreateTextTrigger(new Vector2(2f, 11f), new[] { ("Судя по этим записям этот мир действительно хорош.\n Жаль я его совершенно не помню. ", 5000),
            ("Вот только почерк мне кажется уж очень знакомым...", 4000)});

            scene.Map.At(13, 2).SetTag("door2");
            CreateDoorOpenTrigger(new Vector2(2, 12), "door2");

            scene.Map.At(18, 11).SetTag("door3");
            CreateDoorOpenTrigger(new Vector2(10, 18), "door3");

            CreateTextTrigger(new Vector2(9f, 18f), new[] { ("Какая большая крыса. Зараза.", 3000),
            ("Надеюсь она единственная.", 4000)});

            scene.Map.At(20, 16).SetTag("door4");
            CreateDoorOpenTrigger(new Vector2(16, 19), "door4");

            CreateTextTrigger(new Vector2(12f, 19f), new[] { ("Крысы, не видят в сквозь стекло. Глупые твари.", 5000),
            ("Тем не менее комната выглядит подазрительно, нужно быть осторожнее.", 4000)});

            CreateTextTrigger(new Vector2(12f, 18f), new[] { ("Дьявольщина! Так и знал", 5000)});

            scene.Map.At(29, 5).SetTag("door5");
            CreateDoorOpenTrigger(new Vector2(6, 29), "door5");

            scene.Map.At(13, 14).SetTag("door6");
            CreateDoorOpenTrigger(new Vector2(14, 14), "door6");

            CreateTextTrigger(new Vector2(14f, 12f), new[] { ("Чтож приятная находка.", 5000) });

            scene.Map.At(17, 22).SetTag("door7");
            CreateDoorOpenTrigger(new Vector2(21, 17), "door7");

            scene.Map.At(9, 29).SetTag("door8");
            CreateDoorOpenTrigger(new Vector2(30, 9), "door8");

            scene.Map.At(4, 45).SetTag("door9");
            CreateDoorOpenTrigger(new Vector2(45, 3), "door9");

            scene.Map.At(4, 10).SetTag("doora");
            CreateDoorOpenTrigger(new Vector2(10, 3), "doora");

            scene.Map.At(13, 53).SetTag("doorc");
            CreateDoorOpenTrigger(new Vector2(53, 12), "doorc");

            scene.Map.At(19, 53).SetTag("doorf");
            CreateDoorOpenTrigger(new Vector2(53, 18), "doorf");

            scene.Map.At(30, 29).SetTag("doorl");
            CreateDoorOpenTrigger(new Vector2(30, 30), "doorl");

            scene.Map.At(29, 41).SetTag("doori");
            CreateDoorOpenTrigger(new Vector2(41, 30), "doori");

            scene.Map.At(4, 52).SetTag("doore");
            CreateDoorOpenTrigger(new Vector2(52, 5), "doore");

            scene.Map.At(27, 53).SetTag("doorg");
            CreateDoorOpenTrigger(new Vector2(53, 26), "doorg");

            scene.AddObject(Note.Create(new Vector2(8.5f, 12.5f), "Гномы - сильные, но маленькие существа, немного глупее людей,\n" +
                "но зато гораздо усерднее последних, из-за чего добились  немалого\nтехнического прогресса, который, впрочем, недалеко (10-15 лет)\n" +
                "уходит от людского. Часто враждуют с гоблинами, из-за того, \nчто не могут поделить пещеры."));
            scene.AddObject(Note.Create(new Vector2(1.5f, 27.5f), "Гоблины - относительно слабые существа, которые, nне сказать чтобы\nглупее чем" +
                " люди или гномы. Проблема в том, \nчто их культурное и социальное развитие находится на \nнизком уровне относительно людского " +
                "и гномского, из-за чего гоблины \nпоследним кажутся примитивными и агрессивными существами. \nУ гоблинов хорошие отношения с" +
                " троллями, потому что гоблины \nвыступают их “юридическими представителями” - продают ресурсы, \nкоторые добывают тролли."));
            scene.AddObject(Note.Create(new Vector2(4.5f, 27.5f), "Тролли - самые сильные существа, но очень глупые и доверчивые,\nчем и пользуются" +
                "гоблины, использующие троллей в качестве тяжёлой силы.\nПо натуре добры и доверчивы, ни с кем не враждуют."));
            scene.AddObject(Note.Create(new Vector2(23.5f, 30.5f), "Кажется тут выход."));
            scene.AddObject(Note.Create(new Vector2(23.5f, 19.5f), "Так же существуют очень малочисленные, но не менее разумные рассы:\nнапример " +
                "Драконы или Большие кошки (последние \n" +
                "исчезли не без участия человека, поскольку считались \n" +
                "и до сих пор считаются (не без оснований) пособниками \n" +
                "злых богов), однако, надежд на восстановление их рода \nуже нет, так как никого почти не осталось."));


            while (engine.Update(scene))
            {
            }
        }

        private static void CreateTextTrigger(Vector2 pos, (string msg, int duration)[] texts )
        {
            Trigger.AddTrigger(pos, (scene) =>
            {
                scene.Player.CurrentMonologue = new Monologue(texts);
            }, false);
        }

        private static void CreateDoorOpenTrigger(Vector2 pos, string tag)
        {
            Trigger.AddTrigger(pos, (scene) =>
            {      
                Map.GetCellByTag(tag).StartAnimatiom(() => { Map.GetCellByTag(tag).Type = MapCellType.Empty; });
            });
        }
        private class MapTextureStorage
        {
            private readonly Sprite ceilingTexture;
            private readonly Sprite wallTexture;
            private readonly Sprite floorTexture;
            private readonly Sprite windowTexture;
            private readonly Animation doorAnimation;
            private readonly Sprite bedTexture;

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite windowTexture, Sprite bad, Animation doorAnimation)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.windowTexture = windowTexture;
                this.doorAnimation = doorAnimation;
                this.bedTexture = bad;
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    'd' => new MapCell(MapCellType.Door, doorAnimation, wallTexture, ceilingTexture),
                    '#' => new MapCell(MapCellType.Wall, wallTexture, wallTexture, ceilingTexture),
                    '$' => new MapCell(MapCellType.Wall, Sprite.Load("./sprites/wall2.png"), wallTexture, ceilingTexture),
                    '%' => new MapCell(MapCellType.Wall, Sprite.Load("./sprites/ceiling2.png"), wallTexture, ceilingTexture),
                    '@' => new MapCell(MapCellType.Wall, Sprite.Load("./sprites/books.png"), wallTexture, ceilingTexture),
                    'o' => new MapCell(MapCellType.Window, windowTexture, floorTexture, ceilingTexture),
                    'b' => new MapCell(MapCellType.TransparentObj, bedTexture, floorTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }

            
        }
    }
}