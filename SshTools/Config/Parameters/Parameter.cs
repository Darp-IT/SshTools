using System;
using System.Collections.Generic;
using SshTools.Config.Util;

namespace SshTools.Config.Parameters
{
    public sealed class Parameter<T> : IParameter<T>
    {
        private T _argument;
        
        public Keyword<T> Keyword { get; set; }
        object IParameter.Argument
        {
            get => Argument;
            set => Argument = (T)value;
        }

        Keyword IParameter.Keyword => Keyword;

        public T Argument
        {
            get => _argument;
            set
            {
                if (_argument is IConnectable oldConn) 
                    oldConn.Disconnect();
                _argument = value;
                if (value is IConnectable newConn)
                    newConn.Connect(this);
            }
        }

        public CommentList Comments { get; private set; } = new CommentList();
        public ParameterAppearance ParameterAppearance { get; }
        
        internal Parameter(Keyword<T> keyword, T argument, ParameterAppearance appearance)
        {
            ParameterAppearance = appearance;
            Keyword = keyword;
            Argument = argument;
        }
        
        public string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var lines = new List<string>();
            if (!options.HasFlag(SerializeConfigOptions.STRIP_COMMENTS))
                lines.AddRange(ParameterAppearance.GenerateComments(Comments));
            var line = options.HasFlag(SerializeConfigOptions.TRIM_FRONT) 
                ? ParameterAppearance.DefaultFrontSpacing 
                : ParameterAppearance.SpacingFront;
            line += options.HasFlag(SerializeConfigOptions.USE_CAMEL_CASE)
                ? Keyword.Name
                : ParameterAppearance.Keyword;
            line += options.HasFlag(SerializeConfigOptions.USE_DEFAULT_SEPARATOR)
                ? ParameterAppearance.DefaultSeparator
                : ParameterAppearance.Separator;
            var quoted = options.HasFlag(SerializeConfigOptions.USE_QUOTING) || ParameterAppearance.IsQuoted;
            if (quoted) 
                line += "\"";
            line += Keyword.SerializeArgument(Argument, options);
            if (quoted)
                line += "\"";
            if (!options.HasFlag(SerializeConfigOptions.TRIM_BACK)) 
                line += ParameterAppearance.SpacingBack;
            lines.Add(line);
            return string.Join(Environment.NewLine, lines);
        }

        public ILine Clone() =>
            new Parameter<T>(
                Keyword,
                Argument is ICloneable cloneable
                    ? (T) cloneable.Clone()
                    : Argument,
                (ParameterAppearance) ParameterAppearance.Clone())
            {
                Comments = Comments
            };

        public override string ToString() => $"({Keyword}={Argument})";
        object ICloneable.Clone() => Clone();
    }
}