using System.Numerics;

namespace objects.Monsters
{
    public abstract class BaseMonster : AnimatedObject
    {
        public BaseMonster(Vector2 position, Vector2 size, float directionAngle, int health) : base(position, size, directionAngle)
        {
            Health = health;
        }

        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public void ReceiveDamage(int damage)
        {
            Health -= damage;
        }
    }
}