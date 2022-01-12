namespace SshTools.Settings
{
    public interface ISetting<out T> : ISetting
    {
        new T Key { get; }
    }
}