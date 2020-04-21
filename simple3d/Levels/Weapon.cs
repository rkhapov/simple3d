using simple3d.Drawing;

namespace simple3d.Levels
{
    public abstract class Weapon
    {
        public abstract Sprite Sprite { get; }
        public abstract void UpdateAnimation(float elapsedMilliseconds);
    }
}