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
        public const string ConfigWithTwoNodes =
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
    }
}