using System;
using SshTools.Config.Util;

namespace SshTools.Config.Parameters
{
    public class Comment : ILine
    {
        public string Spacing { get; }
        public string Argument { get; }

        internal Comment(string comment, string spacing = "")
        {
            Argument = comment;
            Spacing = spacing;
        }
        
        public string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT) => GenerateComment();
        public string GenerateComment() => string.IsNullOrEmpty(Argument)
            ? Spacing
            : Spacing + "#" + Argument;
        public override string ToString() => $"Comment={GenerateComment()}";
        public ILine Clone() => new Comment(Argument, Spacing);
        object ICloneable.Clone() => Clone();
    }
}