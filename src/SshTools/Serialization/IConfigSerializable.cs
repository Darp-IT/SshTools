namespace SshTools.Serialization
{
    public interface IConfigSerializable
    {
        string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT);
    }
}