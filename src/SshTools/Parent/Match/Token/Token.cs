using System;
using System.Linq;
using SshTools.Settings;

namespace SshTools.Parent.Match.Token
{
    public partial class Token : IKeyedSetting<char>
    {
        //-----------------------------------------------------------------------//
        //                                Logic
        //-----------------------------------------------------------------------//
        public static Token[] Values => typeof(Token)
            .GetFields()
            .Select(f => f.GetValue(null))
            .Where(v => v is Token)
            .Cast<Token>()
            .ToArray();

        public char Key { get; }
        public Func<MatchingContext, string> Apply { get; }

        public Token(char key, Func<MatchingContext, string> func) =>
            (Key, Apply) = (key, func);

        public override string ToString() => '%' + Key.ToString();
        object IKeyedSetting.Key => Key;
        Type IKeyedSetting.Type => typeof(Token);
        char IKeyedSetting<char>.Key => Key;
    }
}