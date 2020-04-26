using System;
using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Monsters
{
    public class Ded : BaseMonster
    {
        public Ded(Vector2 position, Animation currentAnimation) : base(position, Vector2.One, 0, 60)
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

        public static Ded Create(Vector2 position)
        {
            return new Ded(position, Animation.FromSingleSprite(Sprite.Load("./sprites/ded.png")));
        }

        protected override int ViewDistance => 50;
        protected override float MoveSpeed => 1434;
        public override Animation CurrentAnimation { get; }
    }
}