using System;
using System.Numerics;
using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Collectables
{
    public class ManaPotion : BaseCollectable
    {
        public ManaPotion(Vector2 position, Animation animation, ISound collectionSound) : base(position, animation, collectionSound)
        {
        }

        public override string Name => "Зелье Маны";
        protected override void OnCollect(Scene scene)
        {
            scene.Player.SpellPoints += Math.Min(7, scene.Player.MaxSpellPoints - scene.Player.SpellPoints);
        }
        
        public static ManaPotion Create(Vector2 position)
        {
            var animation =
                Animation.FromSingleSprite(
                    ResourceCachedLoader.Instance.GetSprite("./sprites/collectables/manapotion.png"));
            var collectingSound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.CollectionSoundPath);

            return new ManaPotion(position, animation, collectingSound);
        }
    }
}