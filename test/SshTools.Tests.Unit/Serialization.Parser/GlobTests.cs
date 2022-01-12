using FluentAssertions;
using Xunit;
using static SshTools.Serialization.Parser.Globber;

namespace SshTools.Tests.Unit.Serialization.Parser
{
    public class GlobTests
    {
        [Theory]
        [InlineData("*", "laputa", true)]
        [InlineData("lap*", "laputa", true)]
        [InlineData("lap*ta", "laputa", true)]
        [InlineData("laputa*", "laputa", true)]
        [InlineData("lap*", "castle", false)]
        public void Glob_TestAsterixPattern(string pattern, string value, bool outcome) => 
            Glob(pattern, value).Should().Be(outcome);

        [Theory]
        [InlineData("lap?ta", "laputa", true)]
        [InlineData("lap?ta", "lap.ta", true)]
        [InlineData("laputa?", "laputa1", true)]
        [InlineData("laputa?", "laputa", false)]
        [InlineData("lap?ta", "lapta", false)]
        [InlineData("lap?ta", "lapuuta", false)]
        [InlineData("lap?ta", "castle", false)]
        public void Glob_TestQuestionMarkPattern(string pattern, string value, bool outcome) => 
            Glob(pattern, value).Should().Be(outcome);

        [Theory]
        [InlineData("laputa,castle", "laputa", true)]
        [InlineData("castle,in,the,sky", "laputa", false)]
        public void Glob_TestCommaSeparatedListPattern(string pattern, string value, bool outcome) => 
            Glob(pattern, value).Should().Be(outcome);

        [Theory]
        [InlineData("!*.dialup.example.com,*.example.com", "www.example.com", true)]
        [InlineData("!*.dialup.example.com,*.example.com", "www.test.example.com", true)]
        [InlineData("!*.dialup.example.com,*.example.com", "www.dialup.example.com", false)]
        [InlineData("*.example.com,!*.dialup.example.com", "www.dialup.example.com", false)]
        public void Glob_TestNegatedListPattern(string pattern, string value, bool outcome) => 
            Glob(pattern, value).Should().Be(outcome);

        [Theory]
        [InlineData("example", "example1", false)]
        [InlineData("* !test", "abc", true)]
        [InlineData("* !test", "test", false)]
        public void Glob_TestWholeString(string pattern, string value, bool outcome) => 
            Glob(pattern, value).Should().Be(outcome);

        [Theory]
        [InlineData("example", "", true)]
        [InlineData("* !test", "", true)]
        [InlineData("* !test", null, true)]
        public void Glob_TestEmptyInput(string pattern, string value, bool outcome) => 
            Glob(pattern, value).Should().Be(outcome);
    }
}