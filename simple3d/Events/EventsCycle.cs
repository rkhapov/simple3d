using System.Collections.Generic;
using simple3d.SDL2;

namespace simple3d.Events
{
    public class EventsCycle : IEventsCycle
    {
        private readonly List<IEventsListener> listeners = new List<IEventsListener>();

        public void AddListener(IEventsListener listener)
        {
            if (listener != null)
            {
                listeners.Add(listener);
            }
        }

        public void ProcessEvents()
        {
            while (SDL.SDL_PollEvent(out var e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    ExitRequested = true;

                foreach (var listener in listeners)
                {
                    listener.HandleEvent(e);
                }
            }
        }

        public bool ExitRequested { get; private set; } = false;
    }
}