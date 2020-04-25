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


namespace LevelTest
{
    internal static class LevelTest
    {
        private static void Main(string[] args)
        {
            using var engine = EngineBuilder.BuildEngine25D(new EngineOptions("simple 3d game", 720, 1280, false));
            var resourceLoader = new ResourceCachedLoader();
            var player = new MyPlayer(new Vector2(2.0f, 7.0f), new Vector2(0.3f, 0.3f), MathF.PI);
            var loader = new ResourceCachedLoader();
            var wallTexture = Sprite.Load("./sprites/wall.png");
            var floorTexture = Sprite.Load("./sprites/floor.png");
            var ceilingTexture = Sprite.Load("./sprites/ceiling.png");
         

            var sword = Sword.Create(loader);
            var bow = Bow.Create(loader);
            player.Weapons = new Weapon[] { bow, sword };

            var storage = new MapTextureStorage(ceilingTexture, wallTexture, floorTexture);
            var objects = new IMapObject[]
            {
                GreenLight.Create(loader, new Vector2(2.0f, 4.0f), new Vector2(0, 0), 0),
                GreenLight.Create(loader, new Vector2(6.0f, 4.0f), new Vector2(0, 0), 0),
                new BaseAnimatedMapObj(new Vector2(2f, 2f), new Vector2(), 0f, resourceLoader.GetAnimation("./sprites/lamp")),
                Rat.Create(loader, new Vector2(8.5f, 2.5f), new Vector2(1.0f, 1.0f), 0),
            };
            var map = Map.FromStrings(new[]
            {
                "##############",
                "#............#",
                "#............#",
                "#....###.....#",
                "#....###.....#",
                "#....###.....#",
                "#............#",
                "#............#",
                "##############"
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
      

            public MapTextureStorage(Sprite ceilingTexture, Sprite wallTexture, Sprite floorTexture)
            {
                this.ceilingTexture = ceilingTexture;
                this.wallTexture = wallTexture;
                this.floorTexture = floorTexture;
       
            }

            public MapCell GetCellByChar(char c)
            {
                return c switch
                {
                    '#' => new MapCell(MapCellType.Wall, wallTexture, wallTexture, ceilingTexture),
                    
                    _ => new MapCell(MapCellType.Empty, floorTexture, floorTexture, ceilingTexture)
                };
            }
        }
    }
}
