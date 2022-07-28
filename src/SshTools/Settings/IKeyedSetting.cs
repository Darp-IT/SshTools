using System;

namespace SshTools.Settings
{
    public interface IKeyedSetting
    {
        object Key { get; }
        Type Type { get; }
    }
}