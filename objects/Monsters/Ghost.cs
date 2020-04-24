using System;
using System.Linq;
using System.Numerics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;
using simple3d.MathUtils;

namespace objects.Monsters
{
    //TODO: fix
    /*
    public class Ghost : BaseAnimatedStaticMapObject
    {
        public Ghost(Vector2 position, Vector2 size, float directionAngle, Animation animation) : base(position, size, directionAngle, animation)
        {
        }

        private const float MoveSpeed = 0.001f;

        public override void OnShoot(Scene scene, ShootingWeapon weapon)
        {
            Console.WriteLine($"{this} has been hit");
            scene.RemoveObject(this);
        }

        public override void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
            scene.RemoveObject(this);
            base.OnLeftMeleeAttack(scene, weapon);
        }

        public override void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
            scene.RemoveObject(this);
            base.OnRightMeleeAttack(scene, weapon);
        }

        public static Ghost Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            var animation = loader.GetAnimation("./animations/ghost");

            return new Ghost(position, size, directionAngle, animation);
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            
        }
    }
*/
}