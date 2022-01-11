using FluentAssertions;
using FluentAssertions.Primitives;

namespace SshTools.Tests.Unit
{
	public static class TestExtensions
	{
		public static AndConstraint<StringAssertions> BeIgnoreEnvironmentLineBreaks(this StringAssertions stringAssertions,
			string expected, string because = "", params object[] becauseArgs)
		{
			var subject = stringAssertions.Subject.Replace("\r\n", "\n");
			return subject.Should().Be(expected.Replace("\r\n", "\n"), because, becauseArgs);
		}
	}
}