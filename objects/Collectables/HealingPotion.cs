using System.Numerics;
using musics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Collectables
{
    public class HealingPotion : BaseCollectable
    {
        public HealingPotion(Vector2 position, Animation animation, ISound collectionSound) : base(position, animation, collectionSound)
        {
        }

        public override string Name => "Healing Potion";
        protected override void OnCollect(Scene scene)
        {
        }

        public static HealingPotion Create(Vector2 position)
        {
            var animation =
                Animation.FromSingleSprite(
                    ResourceCachedLoader.Instance.GetSprite("./sprites/collectables/Healing_Potion.png"));
            var collectingSound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.CollectionSoundPath);

            return new HealingPotion(position, animation, collectingSound);
        }
    }
}