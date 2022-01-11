using SshTools.Config.Util;

namespace SshTools.Config.Parameters
{

    public interface ILine : IConfigSerializable, ICloneable<ILine>
    {
        
    }
    
    public interface IParameter : ILine
    {
        Keyword Keyword { get; }
        object Argument { get; set; }
        CommentList Comments { get; }
    }

    public interface IArgParameter<out T> : IParameter
    {
        new T Argument { get; }
    }
    
    public interface IParameter<T> : IArgParameter<T>
    {
        new Keyword<T> Keyword { get; }
        new T Argument { get; set; }
        ParameterAppearance ParameterAppearance { get; }
    }
    
    
}