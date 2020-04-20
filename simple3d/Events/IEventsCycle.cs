namespace simple3d.Events
{
    public interface IEventsCycle
    {
        void AddListener(IEventsListener listener);

        void ProcessEvents();
        
        bool ExitRequested { get; }
    }
}