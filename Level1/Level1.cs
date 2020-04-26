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
    internal static class Level1
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
                "########################################################",
                "#P...#...#......................................#......#",
                "#....#...#......................................#......#",
                "#....#...#......................................#......#",
                "##d#######d#....................................####.###",
                "#......#AHA#...1.......................................#",
                "#......oAHA#...........................................#",
                "#......#AHA#...........................................#",
                "#......#o###...........................................#",
                "#......#...#...........................................#",
                "#..........#...........................................#",
                "#......#############...................................#",
                "#......#3HHH..2.AAA#...................................#",
                "##d###########d########................................#",
                "#........AH#R.......R.#................................#",
                "#........HA#.....o....#................................#",
                "#...oo.....#.....o....#................................#",
                "#....oo....#.....o....d................................#",
                "#..........d0....o....#................................#",
                "#.R........#..........#................................#",
                "############o##%d%#####................................#",
                "#....................R#................................#",
                "#..%......RR..........#................................#",
                "#.......o........%R...#................................#",
                "#.....................#................................#",
                "#R.......$.......oH...#................................#",
                "######................#................................#",
                "#6..5#......$.........#..........................###.###",
                "#.HA.#......R.........#..........................#.....#",
                "#.HA.d................#..........................#.....#",
                "#.HA.#................#..........................#.....#",
                "########################################################",
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
                    'o' => new MapCell(MapCellType.Window, windowTexture, floorTexture, ceilingTexture),
                    'b' => new MapCell(MapCellType.TransparentObj, bedTexture, floorTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }

            
        }
    }
}