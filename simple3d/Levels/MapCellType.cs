using System.Collections.Generic;

namespace simple3d.Levels
{
    public enum MapCellType
    {
        Empty,
        Window,
        TransparentObj,
        Door,
        Wall
    }

    public class CellTypes
    {
        public static HashSet<MapCellType> walkable = new HashSet<MapCellType> { MapCellType.Empty};
        public static HashSet<MapCellType> transparents = new HashSet<MapCellType> { MapCellType.Window, MapCellType.TransparentObj};

    }
   
}