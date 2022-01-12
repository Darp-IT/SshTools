using System;
using SshTools.Line.Parameter.Keyword;
using SshTools.Parent.Match.Criteria;
using SshTools.Parent.Match.Token;
using SshTools.Settings;

namespace SshTools
{
    //https://github.com/openssh/libopenssh/blob/master/ssh/readconf.c
    public static class SshTools
    {
        internal static SshToolsSettings Settings { get; } = new SshToolsSettings();
        
        public static void Configure(Action<SshToolsSettings> action) => action(Settings);

        static SshTools()
        {
            Configure(settings => settings
                .Add(Keyword.Values)
                .Add(Token.Values)
                .Add(Criteria.Values));
        }
    }
}