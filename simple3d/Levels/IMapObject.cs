using System.Numerics;
using simple3d.Drawing;

namespace simple3d.Levels
{
    public interface IMapObject
    {
        Vector2 Position { get; }
        Vector2 Size { get; }
        float DirectionAngle { get; }
        Sprite Sprite { get; }
        void OnWorldUpdate(Scene scene, float elapsedMilliseconds);
    }
}