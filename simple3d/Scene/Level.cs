namespace simple3d.Scene
{
    public class Level
    {
        public Level(Player player, Map map)
        {
            Player = player;
            Map = map;
        }

        public Player Player { get; }
        public Map Map { get; }
    }
}