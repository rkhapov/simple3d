using System;

namespace simple3d.Sounds
{
    public interface IMusic : IDisposable
    {
        void Play(int loopsCount);
    }
}