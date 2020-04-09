namespace simple3d.Scene
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