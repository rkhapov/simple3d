using simple3d.Levels;

namespace simple3d.Physics
{
    public interface IPhysicEngine
    {
        void UpdateObjects(Scene scene, float elapsedMilliseconds);
    }
}