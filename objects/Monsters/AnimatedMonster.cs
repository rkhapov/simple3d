using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Monsters
{
    public abstract class AnimatedObject : BaseObject
    {
        protected AnimatedObject(Vector2 position, Vector2 size, float directionAngle) : base(position, size, directionAngle)
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