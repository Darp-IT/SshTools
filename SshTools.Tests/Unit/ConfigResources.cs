using FluentResults.Extensions.FluentAssertions;
using SshTools.Config.Parents;

namespace SshTools.Tests.Unit
{
    public static class ConfigResources
    {
        
        public const string ConfigWithoutAnything =
            "";
        public const string ConfigWithOnlyOneLinebreak =
            "\n";
        public const string ConfigWithOnlyOneComment =
            "# Test comment :)";
        public const string ConfigWithOnlyANode = 
            "Host host1\n";
        public const string ConfigWithOneNode = 
            "Host host1\n" + 
            "  User user1";
        public const string ConfigWithTwoNodesAndCommentAtTheEnd =
            "Host host1\n" + 
            "  User user1\n" + 
            "Host host2\n" + 
            "  Port 12345   \n";
        public const string ConfigWithParameterAndNodes = 
            "IdentityFile ~/.ssh/id_rsa   \n" +
            "Host host1\n" +
            "  User user1\n" + 
            "Host host2\n" + 
            "  Port 12345   ";
        public const string ConfigWithEveryParameter = 
            "HostName host1\n" +
            "IdentitiesOnly yes\n" +
            "IdentityFile ~/.ssh/id_rsa\n" +
            "User user1\n" +
            "Port 12345\n" +
            "Host host2\n" +
            "Match all";

        public const string ConfigWithUserAtTheStart =
            "User user1\n" +
            "HostName host1";
        
        public static SshConfig DeserializeString(string configString)
        {
            var res = SshConfig.DeserializeString(configString);

            res.Should().BeSuccess();
            return res.Value;
        }
    }
}