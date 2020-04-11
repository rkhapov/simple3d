using simple3d.Drawing;

namespace simple3d.Levels
{
    public interface IMapObject
    {
        float PositionX { get; }
        float PositionY { get; }
        Sprite Sprite { get; }
        void OnWorldUpdate(Scene scene, float elapsedMilliseconds);
    }
}