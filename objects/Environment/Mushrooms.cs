using System.Numerics;
using objects.Monsters;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Environment
{
    public class Mushrooms : AnimatedObject
    {
        public Mushrooms(Vector2 position, Animation currentAnimation) : base(position, Vector2.Zero, 0)
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

        public static Mushrooms Create(Vector2 position)
        {
            return new Mushrooms(position, Animation.FromSingleSprite(Sprite.Load("./sprites/mushrooms.png")));
        }

        protected override int ViewDistance { get; }
        protected override float MoveSpeed { get; }
        public override Animation CurrentAnimation { get; }
    }
}