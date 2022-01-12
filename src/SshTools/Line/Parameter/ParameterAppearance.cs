using System;

namespace SshTools.Line.Parameter
{
    public readonly struct ParameterAppearance : ICloneable
    {
        public const string DefaultFrontSpacing = "  ";
        public const string DefaultSeparator = " ";
        public const bool DefaultUseQuoting = false;
        
        //  Type
        //public CommentList HeaderList { get; }
        public string SpacingFront { get; }
        public string Keyword { get; }
        public string Separator { get; }
        public bool IsQuoted { get; }
        //  Argument
        public string SpacingBack { get; }
        
        
        public ParameterAppearance(string spacingFront, string keyword, string separator, bool isQuoted, string spacingBack)
        {
            //HeaderList = headerList;
            SpacingFront = spacingFront;
            Separator = separator;
            IsQuoted = isQuoted;
            SpacingBack = spacingBack;
            Keyword = keyword;
        }

        public object Clone() => MemberwiseClone();

        public static ParameterAppearance Default(Keyword.Keyword keyword) => 
            new ParameterAppearance(
                //new CommentList(), 
                DefaultFrontSpacing,
                keyword.Name,
                DefaultSeparator,
                DefaultUseQuoting,
                "");
    }
}