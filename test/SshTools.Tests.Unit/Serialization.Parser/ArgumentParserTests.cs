using System;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Parent.Host;
using SshTools.Parent.Match;
using SshTools.Serialization;
using SshTools.Serialization.Parser;
using Xunit;

namespace SshTools.Tests.Unit.Serialization.Parser
{
    public class ArgumentParserTests
    {

        private static void Test_DeserializeAndSerialize<TParser, TArgument>(
            TParser parser,
            Func<TArgument, bool> isExpected,
            bool success,
            string argument)
            where TParser : ArgumentParser<TArgument>
        {
            var res = parser.Deserializer(argument);
            res.Should().BeSuccess(success);
            if (res.IsFailed) return;

            isExpected(res.Value).Should().BeTrue();
            
            var res2 = parser.Serializer(res.Value, SerializeConfigOptions.DEFAULT);
            res2.Should().BeEquivalentTo(argument);
        }

        [Theory]
        [InlineData("test-host", true)]
        [InlineData(null, false)]
        public void Host_DeserializeAndSerialize(string argument, bool success = true) =>
            Test_DeserializeAndSerialize(ArgumentParser.Host, (HostNode v) => v.PatternName == argument, success, argument);
        
        [Theory]
        [InlineData("all", true)]
        [InlineData("host test-host", true)]
        [InlineData("test", false)]
        [InlineData(null, false)]
        public void Match_DeserializeAndSerialize(string argument, bool success = true) =>
            Test_DeserializeAndSerialize(ArgumentParser.Match, (MatchNode v) => v.PatternName == argument, success, argument);
        
        [Theory]
        [InlineData("test-host", true, "test-host")]
        [InlineData(null, false)]
        public void String_DeserializeAndSerialize(string argument, bool success = true, string expected = default) =>
            Test_DeserializeAndSerialize(ArgumentParser.String, (string v) => v == expected, success, argument);
        
        [Theory]
        [InlineData("10", true, 10)]
        [InlineData("10000000000000000000", false)]
        [InlineData("-1", false)]
        [InlineData("test-host", false)]
        [InlineData(null, false)]
        public void UShort_DeserializeAndSerialize(string argument, bool success = true, ushort expected = default) =>
            Test_DeserializeAndSerialize(ArgumentParser.UShort, (ushort v) => v == expected, success, argument);
        
        [Theory]
        [InlineData("yes", true, true)]
        [InlineData("no", true, false)]
        [InlineData("asd", false)]
        [InlineData(null, false)]
        public void YesNo_DeserializeAndSerialize(string argument, bool success = true, bool expected = default) =>
            Test_DeserializeAndSerialize(ArgumentParser.YesNo, (bool v) => v == expected, success, argument);
    }
}