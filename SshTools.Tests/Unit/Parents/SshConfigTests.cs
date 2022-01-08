using FluentAssertions;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using Xunit;

using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parents
{
    public class SshConfigTests
    {
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
    }
}