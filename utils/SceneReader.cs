using System;
using System.Collections.Generic;
using System.Numerics;
using objects;
using objects.Collectables;
using objects.Monsters;
using objects.Weapons;
using simple3d;
using simple3d.Levels;

namespace utils
{
    public static class SceneReader
    {
        static void PrintIntresting(string[] map)
        {
            for (var y = 0; y < map.Length; y++)
            {
                for (var x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] != '#' && map[y][x] != '.' && map[y][x] != 'o')
                        Console.WriteLine(string.Format("{0} - _1={1} _2={2}", map[y][x], x, y));
                }
            }
        }
        public static Scene ReadFromStrings(string[] strings, Func<char, MapCell> mapCellFactory, float startPlayerDirectionAngle)
        {
            SceneReader.PrintIntresting(strings); // /for debug
            var objects = new HashSet<IMapObject>();
            MyPlayer player = null;

            for (var y = 0; y < strings.Length; y++)
            {
                for (var x = 0; x < strings[y].Length; x++)
                {
                    var c = strings[y][x];
                    var mapObject = GetObjectByChar(c, y, x);

                    if (mapObject != null)
                    {
                        objects.Add(mapObject);
                    }

                    if (player == null && c == 'P')
                    {
                        player = new MyPlayer(new Vector2(x + 0.5f, y + 0.5f), new Vector2(0.3f, 0.3f), startPlayerDirectionAngle, 10);
                        var sword = Sword.Create(ResourceCachedLoader.Instance);
                        var bow = Bow.Create(ResourceCachedLoader.Instance);
                        player.Weapons = new Weapon[] {bow, sword};
                        player.Spells = new Spells[] {Spells.FireBall, Spells.ShockBall};
                    }
                }
            }

            var map = Map.FromStrings(strings, mapCellFactory);

            if (player == null)
            {
                throw new NotImplementedException("Scene has no player");
            }

            return new Scene(player, map, objects);
        }

        private static IMapObject GetObjectByChar(char c, int y, int x)
        {
            var position = new Vector2(x + 0.5f, y + 0.5f);

            return c switch
            {
                'R' => (IMapObject) Rat.Create(ResourceCachedLoader.Instance, position, 0),
                'S' => Skeleton.Create(ResourceCachedLoader.Instance, position, 0),
                'L' => Lich.Create(ResourceCachedLoader.Instance, position, 0),
                'H' => HealingPotion.Create(position),
                'M' => ManaPotion.Create(position),
                'A' => ArrowPack.Create(position),
                _ => null
            };
        }
    }
}