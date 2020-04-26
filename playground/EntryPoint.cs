using System;
using System.Numerics;
using musics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using objects;
using objects.Collectables;
using objects.Environment;
using objects.Monsters;
using objects.Weapons;
using simple3d;
using ui;

namespace playground
{
    internal static class EntryPoint
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
            var resourceLoader = ResourceCachedLoader.Instance;
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2);
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var windowTexture = Sprite.Load("./sprites/window.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var bedTexture = Sprite.Load("./sprites/bed.png");
            var sword = Sword.Create(resourceLoader);
            var bow = Bow.Create(resourceLoader);
            var doorAnimation = resourceLoader.GetAnimation("./animations/door");
            player.Weapons = new Weapon[] {sword, bow};
            var backGroundMusic = resourceLoader.GetMusic(MusicResourceHelper.EnvironmentDungeonMusic);
            var objects = new IMapObject[]
            {
                Lich.Create(resourceLoader, new Vector2(6f, 14f), new Vector2(0.6f, 0.6f), 0.0f),
                GreenLight.Create(resourceLoader, new Vector2(8.0f, 8.0f), new Vector2(0, 0), 0),
                HealingPotion.Create(new Vector2(6f, 6f)),
                ArrowPack.Create(new Vector2(7f, 7f)),
                Note.Create(new Vector2(5f, 5f), "о, привет!\nследующая строка\nотвратительно длинная строка с кучей слов капец\nа вот это уже максимум по длине лучше бы его не переступать ага га гус")
            };
            backGroundMusic.Play(-1);
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, windowTexture, bedTexture, doorAnimation);
            var map = Map.FromStrings(new[]
            {
                "###############################",
                "#.........#...................#",
                "#..#..........#...............#",
                "#.........############........#",
                "##........o..........#........#",
                "#b........d..........###......#",
                "#b........o..........#........#",
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

            Trigger.AddTrigger(new Vector2(8f, 5f), () => {
                Console.WriteLine("OPPEEEN THE DOOOOR");
                player.CurrentMonologue = new Monologue(
                    new[] {"привет!\nну и что?", "а\nэто\nвторой монолог лол!"},
                    new[] {3000, 3000}
                    );
                Map.GetCellByTag("door1").StartAnimatiom(() => { Map.GetCellByTag("door1").Type = MapCellType.Empty; });
            });

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
                    'd' => new MapCell(MapCellType.TransparentObj, doorAnimation, wallTexture, ceilingTexture, "door1"),
                    '#' => new MapCell(MapCellType.Wall, wallTexture, wallTexture, ceilingTexture),
                    'o' => new MapCell(MapCellType.Window, windowTexture, floorTexture, ceilingTexture),
                    'b' => new MapCell(MapCellType.TransparentObj, bedTexture, floorTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }
        }
    }
}