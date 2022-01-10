using System.Linq;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using Xunit;

using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parents
{
    public class LineListExLinqTests
    {
        [Theory]
        [InlineData(ConfigWithoutAnything, 0)]
        [InlineData(ConfigWithOnlyANode, 1)]
        [InlineData(ConfigWithEveryParameter, 2)]
        public void Nodes_TestCountOfTypeNode(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var res = config.Nodes();

            res.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, 0)]
        [InlineData(ConfigWithOnlyANode, 1)]
        [InlineData(ConfigWithNodesWithPatterns, 4)]
        public void Hosts_TestCountOfTypeHostNode(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var res = config.Hosts();

            res.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, 0)]
        [InlineData(ConfigWithEveryParameter, 1)]
        public void Matches_TestCountOfTypeMatchNode(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var res = config.Matches();

            res.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithOneNode, 1, 2)]
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd, 3, 5)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning, 3, 5)]
        public void Flatten_TestCounts(string configString, int expectedCount, int expectedFlattenedCount)
        {
            var config = DeserializeString(configString);
            
            var flattened = config.Flatten();

            config.Should().HaveCount(expectedCount);
            flattened.Should().HaveCount(expectedFlattenedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything)]
        [InlineData(ConfigWithOnlyOneLinebreak)]
        [InlineData(ConfigWithOnlyOneComment)]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning)]
        public void Flatten_TestSerializedIsSame(string configString)
        {
            var config = DeserializeString(configString);
            
            var flattened = config.Flatten();
            var serializedString = flattened.Serialize();

            serializedString.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }

        [Theory]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning)]
        public void Collect_TestAfterUsingFlatten(string configString)
        {
            var config = DeserializeString(configString);
            var flattened = config.Flatten().ToList();

            var collected = flattened.Collect().ToList();

            collected.Should().HaveSameCount(config);
        }
        
        [Theory]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning)]
        [InlineData(ConfigWithRandomShit)]
        public void Cloned_TestSameSerialization(string configString)
        {
            var config = DeserializeString(configString);

            var clonedConfig = config.Cloned().ToConfig();
            var serializedClone = clonedConfig.Serialize();

            serializedClone.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }

        private static SshConfig GetConfigWithHost()
        {
            var config = new SshConfig();
            var res = config.InsertHost(0, "name");

            res.Should().BeSuccess();
            return config;
        }
        
        [Fact]
        public void Cloned_TestIndependence()
        {
            const string testUser = "this-is-a-long-test-user";
            const ushort testPort = 24632;
            const string comment = "comment";
            var config = GetConfigWithHost();
            
            var clonedConfig = config.Cloned().ToConfig();
            clonedConfig.User = testUser;
            clonedConfig.Hosts().First().Port = testPort;
            clonedConfig.Hosts().First().Comments.Add(comment);

            config.User.Should().NotBe(testUser);
            config.Hosts().First().Port.Should().NotBe(testPort);
            config.Hosts().First().Comments.Should().HaveCount(0);
        }

        [Fact]
        public void Compiled_TestIndependence()
        {
            const string testUser = "this-is-a-long-test-user";
            const ushort testPort = 24632;
            const string comment = "comment";
            var config = GetConfigWithHost();
            
            var compiledConfig = config.Compiled().ToConfig();
            compiledConfig.User = testUser;
            compiledConfig.Hosts().First().Port = testPort;
            compiledConfig.Hosts().First().Comments.Add(comment);

            config.User.Should().NotBe(testUser);
            config.Hosts().First().Port.Should().NotBe(testPort);
            config.Hosts().First().Comments.Should().HaveCount(0);
        }

        [Theory]
        [InlineData(ConfigWithoutAnything)]
        [InlineData(ConfigWithOnlyOneComment)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning)]
        public void Compiled_TestSerialization(string configString)
        {
            var config = DeserializeString(configString);

            var compiledConfig = config.Compiled().ToConfig();
            var serializeString = compiledConfig.Serialize();

            serializeString.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }

        [Theory]
        [InlineData(ConfigWithoutAnything, "test", 0)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning, "host1", 2)]
        [InlineData(ConfigWithNodesWithPatterns, "host1", 3)]
        [InlineData(ConfigWithNodesWithPatterns, "host3", 1)]
        [InlineData(ConfigWithNodesWithPatterns, "host1abc", 1)]
        public void Matching_TestCountOfPatternMatchingShouldBeExpected(string configString, string name, int expectedCount)
        {
            var config = DeserializeString(configString);

            var matching = config.Matching(name);

            matching.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, "test", 0)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning, "host1", 2)]
        [InlineData(ConfigWithNodesWithPatterns, "host1", 1)]
        [InlineData(ConfigWithNodesWithPatterns, "host3", 0)]
        [InlineData(ConfigWithNodesWithPatterns, "host1abc", 0)]
        public void Matching_TestCountOfExactMatchingShouldBeExpected(string configString, string name, int expectedCount)
        {
            var config = DeserializeString(configString);

            var matching = config.Matching(name, MatchingOptions.EXACT);

            matching.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithOnlyOneComment, 0)]
        [InlineData(ConfigWithOneNode, 0)]
        [InlineData(ConfigWithEveryParameter, 1)]
        [InlineData(ConfigWithUserAtTheStart, 1)]
        [InlineData(ConfigWithMultipleParameters, 4)]
        public void WhereParam_TestCountOfHostNameParametersInConfig(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var paramList = config.WhereParam(Keyword.HostName);

            paramList.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithOnlyOneComment, 0)]
        [InlineData(ConfigWithOneNode, 0)]
        [InlineData(ConfigWithEveryParameter, 3)]
        [InlineData(ConfigWithUserAtTheStart, 2)]
        [InlineData(ConfigWithMultipleParameters, 8)]
        public void WhereArg_TestCountOfStringParametersInConfig(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var paramList = config.WhereArg<string>();

            paramList.Should().HaveCount(expectedCount);
        }

        [Fact]
        public void SelectArg_TestContentOfArgumentList()
        {
            var config = DeserializeString(ConfigWithMultipleParameters);

            var arguments = config.SelectArg().ToList();

            arguments.Should().HaveElementAt(0, "user1");
            arguments.Should().HaveElementAt(4, "user3");
            arguments.Should().HaveElementAt(7, "host4");
        }
        
        [Fact]
        public void SelectArg_TestIgnoreLinesThatAreNotParameters()
        {
            var config = DeserializeString(ConfigWithParametersNodesAndComments);

            var arguments = config.SelectArg().ToList();

            arguments.Should().HaveCount(3);
        }
        
        [Fact]
        public void OfParameter_TestIgnoreLinesThatAreNotParameters()
        {
            var config = DeserializeString(ConfigWithParametersNodesAndComments);

            var arguments = config.OfParameter().ToList();

            arguments.Should().HaveCount(3);
        }
    }
}