﻿using System.Numerics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Environment
{
    public class GreenLight : BaseStaticMapObject
    {
        public GreenLight(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
        }

        public static GreenLight Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var sprite = loader.GetSprite("./sprites/greenlight.png");

            return new GreenLight(position, size, directionAngle, sprite);
        }
    }
}