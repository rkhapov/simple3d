using simple3d.Drawing;

namespace simple3d.Levels
{
    public struct MapCell
    {
        public MapCell(MapCellType type, Sprite sprite)
        {
            Type = type;
            Sprite = sprite;
        }

        public MapCellType Type { get; }
        public Sprite Sprite { get; }
    }
}