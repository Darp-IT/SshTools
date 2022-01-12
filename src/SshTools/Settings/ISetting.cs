using System;

namespace SshTools.Settings
{
    public interface ISetting
    {
        object Key { get; }
        Type Type { get; }
    }
}