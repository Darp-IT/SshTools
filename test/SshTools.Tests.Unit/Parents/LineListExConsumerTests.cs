using System;
using System.Collections.Generic;
using FluentAssertions;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using SshTools.Config.Util;
using Xunit;
using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parents
{
    public class LineListExConsumerTests
    {
        [Fact]
        public void ToConfig_TestNaming()
        {
            const string configFile = "file";
            const string newConfigFile = "file1";
            var config = new SshConfig(configFile);

            var newConfig = config.ToConfig(newConfigFile);

            config.FileName.Should().Be(configFile);
            newConfig.FileName.Should().Be(newConfigFile);
        }

        [Theory]
        [InlineData(ConfigWithParametersNodesAndComments)]
        public void ToConfig_TestEqualCountAfterCallingToConfig(string configString)
        {
            var config = DeserializeString(configString);

            var newConfig = config.ToConfig();

            newConfig.Should().HaveSameCount(config);
        }
        
        [Fact]
        public void ToConfig_CanBeCalledOnIEnumerable()
        {
            IEnumerable<ILine> enumerable = new List<ILine>();
            _ = enumerable.ToConfig();
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, 0, 0)]
        [InlineData(ConfigWithOnlyOneComment, 0, 1)]
        [InlineData(ConfigWithEveryParameter, 5, 0)]
        [InlineData(ConfigWithOneNode, 0, 0)]
        [InlineData(ConfigWithMultipleParameters, 2, 0)]
        [InlineData(ConfigWithParametersNodesAndComments, 1, 1)]
        public void ToHost_TestHostsHaveExpectedCount(string configString,
            int expectedLineCount, int expectedCommentCount)
        {
            const string hostName = "name";
            var config = DeserializeString(configString);

            var host = config.ToHost(hostName);

            host.Should().HaveCount(expectedLineCount);
            host.Comments.Should().HaveCount(expectedCommentCount);
        }
        
        [Fact]
        public void ToHost_TestCommentTransferFromOtherNode()
        {
            const string hostName = "name";
            const string newHostName = "name1";
            var host = new HostNode(hostName);
            host.Comments.Add("TestComment");
            
            var newHost = host.ToHost(newHostName);

            newHost.Comments.Should().HaveCount(1);
        }
        
        [Fact]
        public void ToHost_CanBeCalledOnIEnumerable()
        {
            IEnumerable<ILine> enumerable = new List<ILine>();
            _ = enumerable.ToHost("name");
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, "test", 0)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning, "host1", 2)]
        [InlineData(ConfigWithNodesWithPatterns, "host1", 3)]
        [InlineData(ConfigWithNodesWithPatterns, "host3", 1)]
        [InlineData(ConfigWithNodesWithPatterns, "host1abc", 1)]
        public void Find_TestCountOfPatternMatchingShouldBeExpected(string configString, string name, int expectedCount)
        {
            var config = DeserializeString(configString);

            var host = config.Find(name);

            host.Should().HaveCount(expectedCount);
        }
        
        [Fact]
        public void Compile_CanBeCalledOnIEnumerable()
        {
            IEnumerable<ILine> enumerable = new List<ILine>();
            _ = enumerable.Compile();
        }
        
        [Fact]
        public void GetNodes_CanBeCalledOnIEnumerable()
        {
            IEnumerable<ILine> enumerable = new List<ILine>();
            _ = enumerable.GetNodes();
        }
        
        [Fact]
        public void GetHosts_CanBeCalledOnIEnumerable()
        {
            IEnumerable<ILine> enumerable = new List<ILine>();
            _ = enumerable.GetHosts();
        }
        
        [Fact]
        public void GetMatches_CanBeCalledOnIEnumerable()
        {
            IEnumerable<ILine> enumerable = new List<ILine>();
            _ = enumerable.GetMatches();
        }
        
        [Theory]
        [InlineData(SerializeConfigOptions.DEFAULT, "  #   ")] //TODO add more options and not only at the comment
        public void Serialize_TestOptions(SerializeConfigOptions options, string expectedCommentAt1)
        {
            var config = DeserializeString(ConfigWithRandomShit);

            var serialized = config.Serialize(options);
            var lines = serialized.Split(Environment.NewLine);

            lines.Should().HaveCount(9);
            lines.Should().HaveElementAt(1, expectedCommentAt1);

        }
    }
}