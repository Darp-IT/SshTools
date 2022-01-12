namespace SshTools.Settings
{
    public interface IKeyedSetting<out T> : IKeyedSetting
    {
        new T Key { get; }
    }
}