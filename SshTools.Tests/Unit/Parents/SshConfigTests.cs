using System.Linq;
using FluentAssertions;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using Xunit;

using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parents
{
    public class SshConfigTests
    {
        private static SshConfig DeserializeString(string configString, bool successfulParsing = true)
        {
            var res = SshConfig.DeserializeString(configString);
            
            res.IsSuccess.Should().Be(successfulParsing);
            return res.Value;
        }

        
        [Theory]
        [InlineData(ConfigWithoutAnything)]
        [InlineData(ConfigWithOnlyOneLinebreak)]
        [InlineData(ConfigWithOnlyOneComment)]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodes)]
        [InlineData(ConfigWithParameterAndNodes)]
        public void SerializeString_WithoutChangingConfig(string configString)
        {
            var config = DeserializeString(configString);

            var serialized = config.Serialize();

            serialized.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything)]
        [InlineData(ConfigWithOnlyOneLinebreak)]
        [InlineData(ConfigWithOnlyOneComment)]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodes)]
        [InlineData(ConfigWithParameterAndNodes)]
        public void SerializeString_AfterIssuingToConfig(string configString)
        {
            var config = DeserializeString(configString);

            var serialized = config.ToConfig().Serialize();

            serialized.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }
        
        [Theory]
        [InlineData(ConfigWithOneNode, 1, 2)]
        [InlineData(ConfigWithTwoNodes, 2, 4)]
        [InlineData(ConfigWithParameterAndNodes, 3, 5)]
        public void Flatten_TestCounts(string configString, int expectedCount, int expectedFlattenedCount)
        {
            var config = DeserializeString(configString);

            var flattened = config.Flatten();

            config.Should().HaveCount(expectedCount);
            flattened.Should().HaveCount(expectedFlattenedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodes)]
        [InlineData(ConfigWithParameterAndNodes)]
        public void Cloned_TestSameSerialization(string configString)
        {
            var config = DeserializeString(configString);

            var clonedConfig = config.Cloned().ToConfig();
            var serializedClone = clonedConfig.Serialize();

            serializedClone.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }
        
        [Theory]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodes)]
        [InlineData(ConfigWithParameterAndNodes)]
        public void Cloned_TestIndependence(string configString)
        {
            const string testUser = "this-is-a-long-test-user";
            const ushort testPort = 24632;
            var config = DeserializeString(configString);

            var clonedConfig = config.Cloned().ToConfig();
            clonedConfig.User = testUser;
            clonedConfig.Hosts().First().Port = testPort;

            config.User.Should().NotBe(testUser);
            config.Hosts().First().Port.Should().NotBe(testPort);
        }
    }
}