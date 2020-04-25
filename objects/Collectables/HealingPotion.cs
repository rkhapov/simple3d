using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects.Collectables
{
    public class HealingPotion : BaseCollectable
    {
        public HealingPotion(Vector2 position, Vector2 size, float directionAngle, Animation animation, ISound collectionSound) : base(position, size, directionAngle, animation, collectionSound)
        {
        }

        public override string Name => "Healing Potion";
        protected override void OnCollect(Scene scene)
        {
        }
    }
}