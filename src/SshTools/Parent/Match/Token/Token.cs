using System;
using System.Linq;

namespace SshTools.Parent.Match.Token
{
    public partial class Token
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
    }
}