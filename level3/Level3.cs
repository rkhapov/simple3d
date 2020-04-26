using System;
using System.Numerics;
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

namespace level3
{
    public class Level3
    {
        private static void Main(string[] args)
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
                "$$$e$$$$$###############",
                "$P,,,,,,,..........D...#",
                "$$$$$$$$$###############",
            }, textureStorage.GetCellByChar, MathF.PI / 2);

            Trigger.AddTrigger(1, 15, scene =>
            {
                scene.Player.CurrentMonologue = new Monologue(new[]
                {
                    ("Вот и ты маг, что лишил нас магии ", 2000),
                    ("Маг, что изменил порядок планет", 2000),
                    ("Как бы там ни было много времени уже прошло", 2000),
                    ("Ты добился своей цели, переместил нашу планету дальше от солнца", 3000),
                    ("Тем самым лишил нас источника магии, и поддталкнул к пути прогресса", 3000),
                    ("Но вот только ценой чего?", 3000),
                    ("В попытке переместить себя во времени, ты застрял, здезь в своей лаборатории", 3000),
                    ("Чтож да крутится тебе в этой петле времени в наказание за твое нахальсвто и наглость!", 3000),
                });
            });

            var music = loader.GetMusic(MusicResourceHelper.EnvironmentDungeonMusic);
            music.Play(-1);


            while (engine.Update(scene))
            {
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
                        return new MapCell(MapCellType.Empty, wallTexture, floorTexture, ceilingTexture);
                }
            }
        }
    }
}