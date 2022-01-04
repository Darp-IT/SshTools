using SshTools.Config.Parameters;

namespace SshTools.Config.Util
{
    public interface IConnectable
    {
        void Connect(IParameter parameter);
        void Disconnect();
    }
}