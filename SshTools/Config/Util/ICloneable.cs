using System;

namespace SshTools.Config.Util
{
    public interface ICloneable<out T> : ICloneable
    {
        new T Clone();
    }
}