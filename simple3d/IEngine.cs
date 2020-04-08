using System;
using simple3d.Scene;

namespace simple3d
{
    public interface IEngine: IDisposable
    {
        void Run(Level level);
    }
}