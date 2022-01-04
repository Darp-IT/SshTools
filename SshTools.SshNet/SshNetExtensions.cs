using System;
using FluentResults;
using SshTools.Config.Parents;

namespace SshTools.SshNet
{
    public static class SshNetExtensions
    {
        public static Result Connect(this HostNode host) => throw new NotImplementedException();
        public static Result Connect(this SshConfig parent, string hostName) => throw new NotImplementedException();
    }
}