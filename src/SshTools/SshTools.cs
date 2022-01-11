using System;

namespace SshTools
{
    //https://github.com/openssh/libopenssh/blob/master/ssh/readconf.c
    public static class SshTools
    {
        
        internal static SshToolsSettings Settings { get; } = new SshToolsSettings();
        
        public static void Configure(Action<SshToolsSettings> action) => action(Settings);
    }
}