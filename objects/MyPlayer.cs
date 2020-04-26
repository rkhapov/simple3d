using System;
using System.Numerics;
using System.Reflection.Metadata;
using simple3d.Levels;

namespace objects
{
    public class MyPlayer : Player
    {
        public MyPlayer(Vector2 position, Vector2 size, float directionAngle, int startAmountOfArrows) : base(position, size, directionAngle, startAmountOfArrows)
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
            if (Weapon is MeleeWeapon meleeWeapon)
            {
                if (meleeWeapon.State != MeleeWeaponState.BlockLeft)
                    Health -= damage;
            }
            else
                Health -= damage;
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            if (Weapon is MeleeWeapon meleeWeapon)
            {
                if (meleeWeapon.State != MeleeWeaponState.BlockRight)
                    Health -= damage;
            }
            else
                Health -= damage;
        }

        public override void OnShoot(Scene scene, int damage)
        {
            Health -= damage;
        }
    }
}