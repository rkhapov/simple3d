using System;
using System.Linq;
using simple3d.Levels;
using simple3d.MathUtils;

namespace simple3d.Physics
{
    public class PhysicEngine : IPhysicEngine
    {
        public void UpdateObjects(Scene scene, float elapsedMilliseconds)
        {
            /*
            var physicObjects = scene
                .Objects
                .Where(e => e is IPhysicObject physicObject)
                .Cast<IPhysicObject>()
                .ToArray();

            var rotatedVertices = physicObjects
                .Select(o => o.GetRotatedVertices())
                .ToArray();

            foreach (var physicObject in physicObjects)
            {
                physicObject.Position += physicObject.Velocity * elapsedMilliseconds;
            }

            var l = physicObjects.Length;

            for (var i = 0; i < l; i++)
            {
                var vertices1 = rotatedVertices[i];

                for (var j = i + 1; j < l; j++)
                {
                    var vertices2 = rotatedVertices[j];

                    if (GeometryHelper.IsRectanglesIntersects(vertices1, vertices2))
                    {
                        var object1 = physicObjects[i];
                        var object2 = physicObjects[j];

                        object1.OnCollision(object2);
                        object2.OnCollision(object1);

                        vertices1 = rotatedVertices[i] = object1.GetRotatedVertices();
                        vertices2 = rotatedVertices[j] = object2.GetRotatedVertices();

                        if (GeometryHelper.IsRectanglesIntersects(vertices1, vertices2))
                        {
                            throw new InvalidOperationException("Objects mustn collide after calliing of OnCollision");
                        }
                    }
                }
            }
        */
        }
    }
}