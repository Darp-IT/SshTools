using System;

namespace SshTools.Line
{
    public interface ICloneable<out T> : ICloneable
    {
        new T Clone();
    }
}