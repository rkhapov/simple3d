using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects
{
    public abstract class BaseStaticMapObject : IMapObject
    {
        protected BaseStaticMapObject(Vector2 position, Vector2 size, float directionAngle, Sprite sprite)
        {
            Position = position;
            Sprite = sprite;
            Size = size;
            DirectionAngle = directionAngle;
        }

        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public float DirectionAngle { get; }
        public Sprite Sprite { get; }

        public abstract void OnWorldUpdate(Scene scene, float elapsedMilliseconds);
        public void OnLeftMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnRightMeleeAttack(Scene scene, MeleeWeapon weapon)
        {
        }

        public void OnShoot(Scene scene, ShootingWeapon weapon)
        {
        }
    }
}