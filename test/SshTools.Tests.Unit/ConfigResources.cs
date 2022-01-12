using FluentResults.Extensions.FluentAssertions;
using SshTools.Parent;

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
            "  HostName host-name";
        public const string ConfigWithTwoNodesAndCommentAtTheEnd =
            "Host host1\n" + 
            "  User user1\n" + 
            "Host host2\n" + 
            "  Port 12345   \n";
        public const string ConfigWithTwoNodesAndParameterAtBeginning = 
            "IdentityFile ~/.ssh/id_rsa   \n" +
            "Host host1\n" +
            "  User user1\n" + 
            "Host host2\n" + 
            "  Port 12345   ";
        public const string ConfigWithNodesWithPatterns =
            "Host host1\n" + 
            "  User user\n" +
            "Host host2\n" + 
            "  Port 12345\n" +
            "Host host?\n" +
            "  IdentityFile file.name\n" + 
            "Host host1*\n" + 
            "  IdentitiesOnly yes";
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
        
        public const string ConfigWithMultipleParameters =
            "User user1\n" +
            "HostName host1\n" + 
            "User user2\n" +
            "HostName host2\n" + 
            "User user3\n" +
            "HostName host3\n" + 
            "User user4\n" +
            "HostName host4\n" ;
        
        public const string ConfigWithParametersNodesAndComments =
            "# Comment1\n" +
            "\n" + 
            "IdentitiesOnly yes\n" +
            "Host *\n" + 
            "  User user1\n" +
            "#  Comment for a match\n" +
            "Match user user1\n" + 
            "  Port 123\n" +
            "  HostName name\n" ;
        
        public const string ConfigWithRandomShit =
            "# Comment1   \n" +
            "  #   \n" + 
            "IdentitiesOnly yes  \n" +
            "Host=*\n" + 
            "  User=\"user1\"\n" +
            "#  Comment for a match" +
            "Match=\"user user1\"\n" + 
            "Port \"123\"\n" +
            "     HostName name\n" ;
            
        
        public static SshConfig DeserializeString(string configString)
        {
            var res = SshConfig.DeserializeString(configString);

            res.Should().BeSuccess();
            return res.Value;
        }
    }
}