using System.Numerics;
using objects.Monsters;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Environment
{
    public class StatiSkeleton : AnimatedObject
    {
        public StatiSkeleton(Vector2 position, Animation currentAnimation) : base(position, Vector2.Zero, 0)
        {
            CurrentAnimation = currentAnimation;
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
        }

        public override void OnShoot(Scene scene, int damage)
        {
        }

        public static StatiSkeleton Create(Vector2 position)
        {
            var animation = ResourceCachedLoader.Instance.GetAnimation("./animations/skeleton/static");

            return new StatiSkeleton(position, animation);
        }

        protected override int ViewDistance => 10;
        protected override float MoveSpeed => 10;
        public override Animation CurrentAnimation { get; }
    }
}
