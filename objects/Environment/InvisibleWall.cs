using System.Numerics;
using simple3d;
using simple3d.Drawing;
using simple3d.Levels;

namespace objects.Environment
{
    public class InvisibleWall : BaseStaticMapObject
    {
        public InvisibleWall(Vector2 position, Vector2 size, float directionAngle, Sprite sprite) : base(position, size, directionAngle, new Sprite(new [] {1}, 1,1))
        {
        }

        public override void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            // throw new System.NotImplementedException();
        }
        public static InvisibleWall Create(ResourceCachedLoader loader, Vector2 position, Vector2 size, float directionAngle)
        {
            return new InvisibleWall(position, size, directionAngle, new Sprite(new [] {1}, 1,1));
        }
    }
}