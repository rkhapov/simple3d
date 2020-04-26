using System.Collections.Generic;
using System.Linq;

namespace simple3d.Levels
{
    public class Scene
    {
        private readonly HashSet<IMapObject> mapObjects;
        private object myLock = new object();
        
        public Scene(Player player, Map map, IEnumerable<IMapObject> mapObjects)
        {
            Player = player;
            Map = map;
            this.mapObjects = mapObjects.ToHashSet();
        }

        public Player Player { get; }
        public Map Map { get; }
        public IEnumerable<IMapObject> Objects
        {
            get
            {
                lock (myLock)
                {
                    return mapObjects.ToArray();
                }
            }
        }

        public GameEventsLogger EventsLogger { get; } = new GameEventsLogger();

        public void RemoveObject(IMapObject obj)
        {
            lock (myLock)
            {
                mapObjects.Remove(obj);
            }
        }

        public void AddObject(IMapObject obj)
        {
            lock (myLock)
            {
                mapObjects.Add(obj);
            }
        }
    }
}