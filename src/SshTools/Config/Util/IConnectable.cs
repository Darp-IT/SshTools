using SshTools.Config.Parameters;

namespace SshTools.Config.Util
{
    public interface IConnectable
    {
        void Connect(IParameter line);
        void Disconnect();
        bool IsConnected { get; }
    }
}