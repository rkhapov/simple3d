﻿using System.Collections.Generic;
using System.Linq;

namespace simple3d.Levels
{
    public class Scene
    {
        private readonly HashSet<IMapObject> mapObjects;
        
        public Scene(Player player, Map map, IEnumerable<IMapObject> mapObjects)
        {
            Player = player;
            Map = map;
            this.mapObjects = mapObjects.ToHashSet();
        }

        public Player Player { get; }
        public Map Map { get; }
        public IEnumerable<IMapObject> Objects => mapObjects.ToArray();
        public GameEventsLogger EventsLogger { get; } = new GameEventsLogger();

        public void RemoveObject(IMapObject obj)
        {
            mapObjects.Remove(obj);
        }

        public void AddObject(IMapObject obj)
        {
            mapObjects.Add(obj);
        }
    }
}