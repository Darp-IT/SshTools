using SshTools.Config.Parameters;

namespace SshTools.Config.Util
{
    public interface IConnectable
    {
        void Connect(ILine line);
        void Disconnect();
    }
}