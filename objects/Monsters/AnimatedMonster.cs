using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Monsters
{
    public abstract class AnimatedMonster : BaseMonster
    {
        protected AnimatedMonster(Vector2 position, Vector2 size, float directionAngle, int health) : base(position, size, directionAngle, health)
        {
        }

        public override Sprite Sprite => CurrentAnimation.CurrentFrame;
        
        public abstract Animation CurrentAnimation { get; }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            CurrentAnimation.UpdateFrame(elapsedMilliseconds);
        }
    }
}