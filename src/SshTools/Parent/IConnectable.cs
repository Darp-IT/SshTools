using SshTools.Line.Parameter;

namespace SshTools.Parent
{
    public interface IConnectable
    {
        void Connect(IParameter line);
        void Disconnect();
        bool IsConnected { get; }
    }
}