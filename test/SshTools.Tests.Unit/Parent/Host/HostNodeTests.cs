using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Parent.Host;
using Xunit;

namespace SshTools.Tests.Unit.Parent.Host
{
    public class HostNodeTests
    {
        [Theory]
        [InlineData("host1", true)]
        [InlineData("host1 host2", true)]
        [InlineData("* !hos?1", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Of_TestParsingDifferentPatterns(string patternName, bool expectedIsSuccess)
        {
            var res = HostNode.Of(patternName);

            res.IsSuccess.Should().Be(expectedIsSuccess);
        }
        
        
        [Fact]
        public void Rename_Test()
        {
            const string nameNull = null;
            const string nameName = "host2";
            var host = HostNode.Of("host1").Value;

            var renameResWithName = host.Rename(nameName);
            var renameResWithNull = host.Rename(nameNull);

            renameResWithName.Should().BeSuccess();
            host.PatternName.Should().Be(nameName);
            renameResWithNull.Should().BeFailure();
        }
    }
}