using System;
using System.Numerics;
using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Collectables
{
    public class ArrowPack : BaseCollectable
    {
        public ArrowPack(Vector2 position, Animation animation, ISound collectionSound) : base(position, animation, collectionSound)
        {
        }

        public override string Name => "5 Стрел";

        protected override void OnCollect(Scene scene)
        {
            scene.Player.CurrentAmountOfArrows += Math.Min(5, scene.Player.MaxAmountOfArrows - scene.Player.CurrentAmountOfArrows);
        }

        public static ArrowPack Create(Vector2 position)
        {
            var sound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.CollectionSoundPath);
            var animation =
                Animation.FromSingleSprite(ResourceCachedLoader.Instance.GetSprite("./sprites/collectables/Arrow.png"));

            return new ArrowPack(position, animation, sound);
        }
    }
}