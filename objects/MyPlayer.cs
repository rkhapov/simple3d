using System;
using System.Numerics;
using simple3d.Levels;

namespace objects
{
    public class MyPlayer : Player
    {
        public MyPlayer(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            if (Endurance < MaxEndurance)
            {
                Endurance += elapsedMilliseconds * 0.001f;
                Endurance = MathF.Min(Endurance, MaxEndurance);
            }

            base.OnWorldUpdate(scene, elapsedMilliseconds);
        }

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            Health -= damage;
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            Health -= damage;
        }

        public override void OnShoot(Scene scene, int damage)
        {
            Health -= damage;
        }
    }
}