using System;

namespace SshTools.Serialization
{
    [Flags]
    public enum SerializeConfigOptions
    {
        DEFAULT = 0,
        STRIP_COMMENTS = 1 << 0,
        TRIM_FRONT = 1 << 1,
        USE_CAMEL_CASE = 1 << 2,
        USE_DEFAULT_SEPARATOR = 1 << 3,
        USE_QUOTING = 1 << 4,
        TRIM_BACK = 1 << 5,
        
        INTENDED = TRIM_FRONT | USE_CAMEL_CASE | USE_DEFAULT_SEPARATOR | USE_QUOTING | TRIM_BACK
    }
}