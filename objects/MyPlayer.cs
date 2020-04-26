using System;
using System.Numerics;
using System.Reflection.Metadata;
using musics;
using objects.Weapons;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.Sounds;

namespace objects
{
    public class MyPlayer : Player
    {
        public MyPlayer(Vector2 position, Vector2 size, float directionAngle, int startAmountOfArrows) : base(position, size, directionAngle, startAmountOfArrows)
        {
            defenceSound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.PlayerDefence);
            hitSound = ResourceCachedLoader.Instance.GetSound(MusicResourceHelper.PlayerDamage);
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
        
        
        private readonly ISound hitSound;
        private readonly ISound defenceSound;

        public override void OnLeftMeleeAttack(Scene scene, int damage)
        {
            if (!(Weapon is MeleeWeapon meleeWeapon) || meleeWeapon.State != MeleeWeaponState.BlockLeft)
            {
                DoHit(scene, damage);
                return;
            }

            defenceSound.Play(0);
            scene.EventsLogger.SuccessfullyDefence("Игрок");
        }

        public override void OnRightMeleeAttack(Scene scene, int damage)
        {
            if (!(Weapon is MeleeWeapon meleeWeapon) || meleeWeapon.State != MeleeWeaponState.BlockRight)
            {
                DoHit(scene, damage);
                return;
            }

            defenceSound.Play(0);
            scene.EventsLogger.SuccessfullyDefence("Игрок");
        }

        public override void OnShoot(Scene scene, int damage)
        {
            DoHit(scene, damage);
        }

        private void DoHit(Scene scene, int damage)
        {
            Health -= damage;
            hitSound.Play(0);
            scene.EventsLogger.MonsterHit("Игрок", damage);
        }

        public override void DoMagic(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
        }
    }
}