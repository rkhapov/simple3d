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
using level2;
using utils;
namespace Level1
{
    public static class Level1
    {
   
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
                "#P..A#...#H.....R.R#........#......#...........R#@.AAA.@",
                "#....#...#.........#........#......#..z........A#@.HHH.@",
                "#.1..#...#a........##ooooooo#....g...........9.H#@.....@",
                "##d#######d##........z.....z.......#.....####d######d###",
                "#......#AHz@#......................#.R.R.#..........e..#",
                "#.z....oAHA@#......................#.....#.H@......@H..#",
                "#......#AHA@#.....ooo..............#..z..#........R....#",
                "#......#o###.....zo.oz.......%######.....#......@....R.#",
                "#......#..R#......ooo......p.d8...R......#...A..R......#",
                "#....z.....#.AA..............%######.....#............A#",
                "#......#############............o..#..z..#..@......@...#",
                "#.2....#.HHH..R.AAA#..........z.o..#A...H#..H........c.#",
                "##d###########d#######################.##############d##",
                "#.....z..AH#R.5..R..RH#.H..#######.............#.......#",
                "#........HA#.....o....#.......%......@...@.....#.......#",
                "#...@@.....#.....o....#....#.....@...@H..@R....o.R.#####",
                "#.z..@@....#..z..o...7d....#.....@R..@...@.....#.....R.#",
                "#.........3d.....o....#....o...................#.....f.#",
                "#.R.......H#....4....H#...R##########################d##",
                "############o###d###########@J......z........z.......z.#",
                "#.....H..............R#AAHH########..###################",
                "#..%.......R..........#....%...........#....H..........#",
                "#H......$........%R...#.............z..#.z....######..H#",
                "#.....................$.........AA.....#..#.....#RH#...#",
                "$R.......$.......oH..@#..z.............#..#..z..#..#.z.#",
                "######...............@#HHH.............#...........#RgR#",
                "@....#......$.........#$$$$$$$$.....z..#.z..#########d##",
                "@.HA.#......R........#$.AAAHHH$........#....#R...#.HH..@",
                "@@HA.d6..............#$.......$.......A##d###..z.#.zHH.@",
                "@.HAz#....@@@........#$.k....dl..........i.......oAAHHt@",
                "@@@@@###$##%###$##%###$$E$$$$$$$##################@@@@@@",
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

            scene.AddObject(StaticLich.Create(new Vector2(22, 2f)));
            scene.AddObject(StaticLich.Create(new Vector2(24, 2f)));
            scene.AddObject(StaticLich.Create(new Vector2(26, 2f)));
            scene.AddObject(StaticRat.Create(new Vector2(33.5f, 11.5f)));
            scene.AddObject(StaticRat.Create(new Vector2(33.5f, 12.5f)));
            scene.AddObject(StatiSkeleton.Create(new Vector2(19.5f, 8.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(38.5f, 2.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(21.5f, 4.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(27.5f, 4.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(10.5f, 5.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(2.5f, 6.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(44.5f, 6.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(51.5f, 6.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(38.5f, 7.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(17.5f, 8.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(21.5f, 8.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(48.5f, 8.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(5.5f, 10.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(38.5f, 11.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(44.5f, 11.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(51.5f, 11.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(6.5f, 14.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(2.5f, 17.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(14.5f, 17.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(36.5f, 20.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(45.5f, 20.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(53.5f, 23.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(36.5f, 23.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(41.5f, 23.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(25.5f, 25.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(36.5f, 27.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(41.5f, 27.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(47.5f, 29.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(51.5f, 29.5f)));
            scene.AddObject(Lamp1.Create(new Vector2(4.5f, 30.5f)));

            CreateTextTrigger(new Vector2(2f, 5f), new[] { ("Интересно, кто оставил эту записку?", 3000) }); 
            scene.AddObject(Note.Create(new Vector2(2, 8), "Этот мир так чудесен, бузупречная флора и фауна,\n безупречный баланс рас... \n "));
            scene.AddObject(Note.Create(new Vector2(9, 9), "Люди - средние по физическим данным, но самые умные представители\n разумных существ." +
                "Особых конфронтаций ни с кем нет,\n но ксенофобии к другим расам, конечно, достаточно."));
            CreateTextTrigger(new Vector2(2f, 11f), new[] { ("Судя по этим записям этот мир действительно хорош.\n Жаль я его совершенно не помню. ", 5000),
            ("Вот только почерк мне кажется уж очень знакомым...", 4000)});
            CreateTextTrigger(new Vector2(28f, 15f), new[] { ("Библиотека. ", 3000),
            ("Готов спорить, за парой стелажей сидят крысы.", 5000)});

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

            CreateTextTrigger(new Vector2(38f, 12f), new[] { ("Опять эти чертовы крысы!", 5000)});

            CreateTextTrigger(new Vector2(27, 9), new[] { ("Они не двигаются! Чертовщина, какойто выстовочный зал.", 5000),
            ("Но такое чувство, что они наблюдают за мной.", 5000)});
            CreateTextTrigger(new Vector2(30, 30), new[] { ("Хмм я видел это помещение в самом начале.", 5000),
            ("Скорее всего конец этих залов в другой стороне.", 5000)});

            CreateTextTrigger(new Vector2(12f, 18f), new[] { ("Дьявольщина! Так и знал", 5000)});

            scene.Map.At(29, 5).SetTag("door5");
            CreateDoorOpenTrigger(new Vector2(6, 29), "door5");

            scene.Map.At(13, 14).SetTag("door6");
            CreateDoorOpenTrigger(new Vector2(14, 14), "door6");

            CreateTextTrigger(new Vector2(14f, 12f), new[] { ("Чтож приятная находка.", 5000) });
            CreateTextTrigger(new Vector2(31f, 30f), new[] { ("Чую там выход.", 5000), ("Или только начало", 3000) });

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
            scene.AddObject(Note.Create(new Vector2(50.5f, 1.5f), "В целом устройство мира с точки зрения его обитателей ничем не отличается\n" +
                "от привычного: вселенная, звезды, планеты, стандартная физика и химия и т.д.\n" +
                "Однако мир не существует сам по себе: он был создан некими предтечами,\n" +
                " которых часто именуют богами."));
            scene.AddObject(Note.Create(new Vector2(54.5f, 1.5f), "Неизвестно, каким образом боги влияют на реальность: симулируется\n" +
                "ли мир на их компьютерах или же он реален, но технологии богов \n" +
                "настолько развиты и сложны, что мы их даже не можем их распознать. \n" +
                "Неизвестны и их мотивы: мир может быть как их игрушкой, так и важным экспериментом.\n" +
                " Известно только одно: иногда они вмешиваются в жизнь обитателей и иногда \n" +
                "их вмешательство очень даже существенно. Например, боги подарили миру магию."));

            scene.AddObject(Note.Create(new Vector2(10.5f, 5.5f), "Магия - очень важная часть мира.\nПоявилась она так давно, что упоминается ещё\n" +
                "в самых древних документах драконов.\nИ, скорее всего, как только боги дали миру магию, \n" +
                "все разумные расы начали попытки её приручения,\n конечно, не только в благородных целях."));

            scene.AddObject(Note.Create(new Vector2(40.5f, 16.5f), "В целом, расы приуспели в изучении магии: кто-то меньше \n" +
                "(например, драконы и тролли вообще не умеют пользоваться магией;\nпервые в силу природных причин, вторые\n" +
                "потому что банально не хватает мыслительных способностей),\nа кто-то больше (мастерство магов разумных кошек\n" +
                "было настолько велико, что однажды люди начали войну, в надежде \nотбить у кошек магические знания, но людские\n" +
                "армии были повержены всего лишь тридцатью магами кошек, \nсоединивших свои силы, инцидент вошёл в историю как\n" +
                "война осколка луны (из-за способа, которым кошки победили людей) и \nна 312 лет сделал королевства людей практически\n" +
                "полностью подчинёнными кошкам)."));
            scene.AddObject(Note.Create(new Vector2(29.5f, 20.5f), "Магия дала много разумным существам, но и многого лишила. \n" +
                "Люди легко могут лечить болезни и решать с её помощью бытовые и производственные \n" +
                "задачи. Однако, технологический прогресс практически застыл во времени,\n" +
                " остановившись на уровне 1ой эры, т.к. разумным расам \n" +
                "не было нужды развивать науку: все стандартные задачи решала магия, \n" +
                "пусть и не известно каким именно способом.Нет, конечно, разумные \n" +
                "обитатели знают на некотором уровне законы мироздания: они могут \n" +
                "рассказать об устройстве звездной системы в которой живут и даже \n" +
                "напишут уравнение силы гравитации, но не более того."));

            Trigger.AddTrigger(new Vector2(24f, 30f), (scene) =>
            {
                Map.ClearTags();
                Level2.StartOnEngine(engine);
            });

            while (engine.Update(scene))
            {
                if (scene.Player.Health <= 0)
                {
                    Map.ClearTags();
                    StartOnEngine(engine);
                }
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
                    'E' => new MapCell(MapCellType.Wall, Sprite.Load("./sprites/dark/entrance.PNG"), floorTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }

            
        }
    }
}