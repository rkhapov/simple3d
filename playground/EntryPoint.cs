using System;
using System.Numerics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using objects;
using objects.Environment;
using objects.Monsters;
using objects.Weapons;
using simple3d;

namespace playground
{
    internal static class EntryPoint
    {
        private static unsafe void Main(string[] args)
        {
            var options = new EngineOptions("simple 3d game", 720, 1280, true, "./fonts/PressStart2P.ttf");
            using var engine = EngineBuilder.BuildEngine25D(options);
            var resourceLoader = new ResourceCachedLoader();
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2);
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var windowTexture = Sprite.Load("./sprites/window.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var sword = Sword.Create(resourceLoader);
            var bow = Bow.Create(resourceLoader);
            var doorAnimation = resourceLoader.GetAnimation("./animations/door");
            player.Weapon = sword;
            var objects = new IMapObject[]
            {
                Ghost.Create(resourceLoader, new Vector2(7.0f, 7.0f), new Vector2(0.5f, 0.5f), 0.0f),
                GreenLight.Create(resourceLoader, new Vector2(8.0f, 8.0f), new Vector2(0, 0), 0),
            };
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, windowTexture, doorAnimation);
            var map = Map.FromStrings(new[]
            {
                "###############################",
                "#.........#...................#",
                "#..#..........#...............#",
                "#.........############........#",
                "#.........o..........#........#",
                "#.........d..........###......#",
                "#.....o...o..........#........#",
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

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite windowTexture, Animation doorAnimation)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.windowTexture = windowTexture;
                this.doorAnimation = doorAnimation;
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    'd' => new MapCell(MapCellType.Window, doorAnimation, wallTexture, ceilingTexture, "door1"),
                    '#' => new MapCell(MapCellType.Wall, wallTexture, wallTexture, ceilingTexture),
                    'o' => new MapCell(MapCellType.Window, windowTexture, floorTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }
        }
    }
}