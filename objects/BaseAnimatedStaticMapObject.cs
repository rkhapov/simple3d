using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects
{
    public abstract class BaseAnimatedStaticMapObject : IMapObject
    {
        private readonly Animation animation;

        protected BaseAnimatedStaticMapObject(Vector2 position, Vector2 size, float directionAngle, Animation animation)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
            this.animation = animation;
        }

        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public float DirectionAngle { get; }
        public Sprite Sprite => animation.CurrentFrame;

        public virtual void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            animation.UpdateFrame(elapsedMilliseconds);
        }

        public virtual void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public virtual void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public abstract void OnShoot(Scene scene, ShootingWeapon weapon);
    }
}