using System.Numerics;
using objects.Monsters;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Environment
{
    public class Lamp1 : AnimatedObject
    {
        public Lamp1(Vector2 position, Animation currentAnimation) : base(position, Vector2.Zero, 0)
        {
            CurrentAnimation = currentAnimation;
        }

        public static Lamp1 Create(Vector2 position)
        {
            var animation = ResourceCachedLoader.Instance.GetAnimation("./animations/lamp");
            return new Lamp1(position, animation);
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

        protected override int ViewDistance => 100;
        protected override float MoveSpeed => 100;
        public override Animation CurrentAnimation { get; }
    }
}