﻿using System.Numerics;
using objects.Monsters;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Environment
{
    public class StaticRat : AnimatedObject
    {
        public StaticRat(Vector2 position, Animation currentAnimation) : base(position, Vector2.Zero, 0)
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

        public static StaticRat Create(Vector2 position)
        {
            var animation = ResourceCachedLoader.Instance.GetAnimation("./animations/rat/static");

            return new StaticRat(position, animation);
        }

        protected override int ViewDistance => 10;
        protected override float MoveSpeed => 10;
        public override Animation CurrentAnimation { get; }
    }
}
