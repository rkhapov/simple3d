using simple3d.SDL2;

namespace simple3d.Events
{
    public interface IEventsListener
    {
        void HandleEvent(SDL.SDL_Event @event);
    }
}