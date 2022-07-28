using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;

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
		
		public static AndWhichConstraint<ResultAssertions<T>, Result<T>> BeSuccess<T>(
			this ResultAssertions<T> resultAssertions,
			bool expected,
			string because = "",
			params object[] becauseArgs) =>
			expected
				? resultAssertions.BeSuccess(because, becauseArgs)
				: resultAssertions.BeFailure(because, becauseArgs);

		public static AndWhichConstraint<ResultAssertions, Result> BeSuccess(
			this ResultAssertions resultAssertions,
			bool expected,
			string because = "",
			params object[] becauseArgs)
		{
			Execute.Assertion
				.BecauseOf(because, becauseArgs)
				.Given(() => resultAssertions.Subject.IsSuccess == expected)
				.ForCondition(actualIsSuccess => actualIsSuccess)
				.FailWith($"Expected result.IsSuccess be {{0}}, but is failed because of '{{1}}'",
					expected, resultAssertions.Subject.Errors);
			return new AndWhichConstraint<ResultAssertions, Result>(resultAssertions, resultAssertions.Subject);
		}
	}
}