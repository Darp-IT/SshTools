namespace SshTools.Line.Parameter
{
    public interface IArgParameter<out T> : IParameter
    {
        new T Argument { get; }
    }
}