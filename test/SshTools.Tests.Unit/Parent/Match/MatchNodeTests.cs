using FluentResults.Extensions.FluentAssertions;
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

            res.Should().BeSuccess(expectedIsSuccess);
        }
        
        [Theory]
        [InlineData("all", true)]
        [InlineData("user user-name1 user user-name2", true)]
        [InlineData("host *,!hos?1", true)]
        [InlineData("all canonical", true)]
        [InlineData("all final", true)]
        [InlineData("all host test-host", false)]
        [InlineData("host test-host all", false)]
        [InlineData("", false)]
        [InlineData("all all", false)]
        [InlineData(null, false)]
        public void Of_TestParsingDifferentPatterns(string patternName, bool expectedIsSuccess)
        {
            var res = MatchNode.Of(patternName);

            res.Should().BeSuccess(expectedIsSuccess);
        }
    }
}