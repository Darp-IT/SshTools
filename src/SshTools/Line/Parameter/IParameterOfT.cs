using SshTools.Line.Parameter.Keyword;

namespace SshTools.Line.Parameter
{
    public interface IParameter<T> : IArgParameter<T>
    {
        new Keyword<T> Keyword { get; }
        new T Argument { get; set; }
        ParameterAppearance ParameterAppearance { get; }
    }
}