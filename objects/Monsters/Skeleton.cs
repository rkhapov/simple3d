using System.Numerics;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Monsters
{
    public class Skeleton : BaseStaticMapObject
    {
        public Skeleton(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, sprite)
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            //nothing
        }
    }
}