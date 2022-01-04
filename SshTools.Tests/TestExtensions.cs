using System;
using FluentResults;
using Xunit;

namespace SshTools.Tests {
	public static class TestExtensions {
		public static void ShouldEqual<T>(this T actual, T expected) => Assert.Equal(expected, actual);

		public static void ShouldBeTheSameAs(this object actual, object expected) => Assert.Same(expected, actual);

		public static void ShouldBeNull(this object actual) => Assert.Null(actual);

		public static void ShouldNotBeNull(this object actual) => Assert.NotNull(actual);

		public static void ShouldBeTrue(this bool b) => Assert.True(b);

		public static void ShouldBeTrue(this bool b, string msg) => Assert.True(b, msg);

		public static void ShouldBeFalse(this bool b) => Assert.False(b);

		public static Exception ShouldBeThrownBy(this Type exceptionType, Action code) => 
			Assert.Throws(exceptionType, code);

		public static T ShouldBe<T>(this object actual) => Assert.IsType<T>(actual);

		public static T ShouldEqual<T>(this Result<T> result, T expected)
		{
			if (result.IsFailed)
				throw new Exception("Result has failed! Errors: " + string.Join(',', result.Errors));
			result.Value.ShouldEqual(expected);
			return result.Value;
		}
	}
}