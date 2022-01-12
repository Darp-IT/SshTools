using FluentAssertions;
using SshTools.Parent.Match;
using Xunit;

namespace SshTools.Tests.Unit.Parent.Match
{
    public class MatchNodeTests
    {
        [Theory]
        [InlineData("all", true)]
        [InlineData("canonical", true)]
        [InlineData("final", true)]
        [InlineData("exec test-exec", true)]
        [InlineData("host host-name", true)]
        [InlineData("originalhost orig-name", true)]
        [InlineData("user user-name", true)]
        [InlineData("localuser local-name", true)]
        [InlineData("exec", false)]
        [InlineData("host", false)]
        [InlineData("originalhost", false)]
        [InlineData("user", false)]
        [InlineData("localuser", false)]
        public void Of_TestParsingOfDifferentCriteria(string matchString, bool expectedIsSuccess)
        {
            var res = MatchNode.Of(matchString);

            res.IsSuccess.Should().Be(expectedIsSuccess);
        }
    }
}