using FluentAssertions;
using SshTools.Parent;
using Xunit;
using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parent
{
    public class SshConfigTests
    {
        [Fact]
        public void TestCreateAFreshConfig()
        {
            var config = new SshConfig();
            Assert.NotNull(config);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything)]
        [InlineData(ConfigWithOnlyOneLinebreak)]
        [InlineData(ConfigWithOnlyOneComment)]
        [InlineData(ConfigWithOnlyANode)]
        [InlineData(ConfigWithOneNode)]
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning)]
        [InlineData(ConfigWithRandomShit)]
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
        [InlineData(ConfigWithTwoNodesAndCommentAtTheEnd)]
        [InlineData(ConfigWithTwoNodesAndParameterAtBeginning)]
        [InlineData(ConfigWithRandomShit)]
        public void SerializeString_AfterIssuingToConfig(string configString)
        {
            var config = DeserializeString(configString);

            var serialized = config.ToConfig().Serialize();

            serialized.Should().BeIgnoreEnvironmentLineBreaks(configString);
        }
    }
}