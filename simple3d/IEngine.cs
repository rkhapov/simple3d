using System;
using simple3d.Levels;

namespace simple3d
{
    public interface IEngine: IDisposable
    {
        bool Update(Scene scene);
    }
}