using System;

namespace simple3d.Sounds
{
    public interface ISound : IDisposable
    {
        void Play(int loopCount);
    }
}