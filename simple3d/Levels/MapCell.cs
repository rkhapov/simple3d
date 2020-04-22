using simple3d.Drawing;

namespace simple3d.Levels
{
    public struct MapCell
    {
        public MapCell(MapCellType type, Sprite wallSprite, Sprite floorSprite, Sprite ceilingSprite)
        {
            Type = type;
            WallSprite = wallSprite;
            CeilingSprite = ceilingSprite;
            FloorSprite = floorSprite;
        }

        public MapCellType Type { get; }
        public Sprite WallSprite { get; }
        public Sprite FloorSprite { get; }
        public Sprite CeilingSprite { get; }
    }
}