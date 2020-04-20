namespace simple3d.Levels
{
    public struct PlayerAction
    {
        public PlayerAction(bool enabled, PlayerActionType type)
        {
            Enabled = enabled;
            Type = type;
        }

        public bool Enabled { get; }
        public PlayerActionType Type { get; }
    }
}