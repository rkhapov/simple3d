using simple3d.Drawing;

namespace simple3d.Levels
{
    public struct MapCell
    {
        public MapCell(MapCellType type, Sprite sprite, Sprite ceilingSprite)
        {
            Type = type;
            Sprite = sprite;
            CeilingSprite = ceilingSprite;
        }

        public MapCellType Type { get; }
        public Sprite Sprite { get; }
        public Sprite CeilingSprite { get; }
    }
}