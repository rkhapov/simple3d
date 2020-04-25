using System.Numerics;
using objects.Monsters;

namespace objects.Weapons
{
    public abstract class Bullet : AnimatedObject
    {
        protected Bullet(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle)
        {
        }
    }
}