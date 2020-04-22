using System;
using System.Numerics;
using simple3d.Builder;
using simple3d.Drawing;
using simple3d.Levels;
using objects;
using objects.Environment;
using objects.Monsters;
using objects.Weapons;

namespace playground
{
    internal static class EntryPoint
    {
        private static unsafe void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine25D(new EngineOptions("simple 3d game", 720, 1280, false));
            var player = new MyPlayer(new Vector2(2.0f, 2.0f), new Vector2(0.3f, 0.3f), MathF.PI / 2);
            var skeletonSprite = Sprite.Load("./sprites/skeleton.png");
            var wallTexture = Sprite.Load("./sprites/greystone.png");
            var floorTexture = Sprite.Load("./sprites/colorstone.png");
            var windowTexture = Sprite.Load("./sprites/window.png");
            var ceilingTexture = Sprite.Load("./sprites/wood.png");
            var greenLightTexture = Sprite.Load("./sprites/greenlight.png");
            var ghostAnimation = Animation.LoadFromDirectory("./animations/ghost");
            var sword = new Sword(
                Animation.LoadFromDirectory("./animations/sword_static"),
                Animation.LoadFromDirectory("./animations/sword_left_attack"),
                Animation.LoadFromDirectory("./animations/sword_right_attack"),
                Animation.LoadFromDirectory("./animations/sword_left_block"),
                Animation.LoadFromDirectory("./animations/sword_right_block"),
                Animation.LoadFromDirectory("./animations/sword_static"));
            var bow = new Bow(
                Animation.LoadFromDirectory("./animations/bow_static"),
                Animation.LoadFromDirectory("./animations/bow_moving"),
                Animation.LoadFromDirectory("./animations/bow_shoot"),
                Sprite.Load("./sprites/arrow.png"));
            player.Weapon = sword;
            var objects = new IMapObject[]
            {
                new Ghost(new Vector2(7.0f, 7.0f), new Vector2(0.5f, 0.5f), 0.0f, ghostAnimation),
                new GreenLight(new Vector2(8.0f, 8.0f), new Vector2(0, 0), 0, greenLightTexture),
            };
            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture, windowTexture);
            var map = Map.FromStrings(new[]
            {
                "###############################",
                "#.........#...................#",
                "#..#..........#...............#",
                "#.........############........#",
                "#.........o..........#........#",
                "#.........o..........###......#",
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

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture, Sprite windowTexture)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
                this.windowTexture = windowTexture;
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    '#' => new MapCell(MapCellType.Wall, wallTexture, ceilingTexture),
                    'o' => new MapCell(MapCellType.Window, windowTexture, ceilingTexture),
                    _ => new MapCell(MapCellType.Empty, floorTexture, ceilingTexture)
                };
            }
        }
    }
}