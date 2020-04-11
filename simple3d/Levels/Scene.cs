using System.Collections.Generic;
using System.Linq;

namespace simple3d.Levels
{
    public class Scene
    {
        private readonly HashSet<IMapObject> mapObjects;
        
        public Scene(PlayerCamera playerCamera, Map map, IEnumerable<IMapObject> mapObjects)
        {
            PlayerCamera = playerCamera;
            Map = map;
            this.mapObjects = mapObjects.ToHashSet();
        }

        public PlayerCamera PlayerCamera { get; }
        public Map Map { get; }
        public IEnumerable<IMapObject> Objects => mapObjects;
    }
}