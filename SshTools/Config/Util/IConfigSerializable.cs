namespace SshTools.Config.Util
{
    public interface IConfigSerializable
    {
        string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT);
    }
}