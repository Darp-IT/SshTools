using System.Collections.Generic;
using FluentAssertions;
using SshTools.Parent.Match.Criteria;
using SshTools.Serialization.Parser;
using Xunit;

namespace SshTools.Tests.Unit.Serialization.Parser
{
    public class MatchStringParserTests
    {
        private static (Criteria, string, string, string) GetTuple(Criteria criteria = null, string spacing = null,
            string argument = null, string spacingBack = null)
            => (criteria, spacing, argument, spacingBack);
        
        public static IEnumerable<object[]> GetMatchSingleCriteriaData()
        {
            yield return new object[] { "all", true, GetTuple(Criteria.All) };
            yield return new object[] { "canonical", true, GetTuple(Criteria.Canonical) };
            yield return new object[] { "final", true, GetTuple(Criteria.Final) };
            yield return new object[] { "all  ", true, GetTuple(Criteria.All, spacingBack:"  ") };
            yield return new object[] { "All", false };
            yield return new object[] { "al", false };
            yield return new object[] { "asd", false };
        }
        
        [Theory]
        [MemberData(nameof(GetMatchSingleCriteriaData))]
        public void Parse_MatchSingles(string argument, bool success,
            (Criteria, string, string, string) expected = default)
        {
            var res = MatchStringParser.Parse(argument);
            
            res.IsSuccess.Should().Be(success);
            if (res.IsFailed) return;
            var list = res.Value;
            list.Should().HaveCount(1);
            list.Should().HaveElementAt(0, expected);
        }

        public static IEnumerable<object[]> GetMatchArgumentCriteriaData()
        {
            // Focus on different criteria
            yield return new object[] { "host test-host", true, GetTuple(Criteria.Host, " ", "test-host") };
            yield return new object[] { "user test-user", true, GetTuple(Criteria.User, " ", "test-user") };
            yield return new object[] { "localuser test-local-user", true, GetTuple(Criteria.LocalUser, " ", "test-local-user") };
            yield return new object[] { "originalhost test-original-host", true, GetTuple(Criteria.OriginalHost, " ", "test-original-host") };
            yield return new object[] { "Host test-host", false };
            yield return new object[] { "hos test-host", false };
            yield return new object[] { "all test-host", false };
            yield return new object[] { "final test-host", false };
            yield return new object[] { "canonical test-host", false };
            // Focus on arguments
            yield return new object[] { "host !test-host", true, GetTuple(Criteria.Host, " ", "!test-host") };
            yield return new object[] { "host *test-host", true, GetTuple(Criteria.Host, " ", "*test-host") };
            yield return new object[] { "host ?test-host", true, GetTuple(Criteria.Host, " ", "?test-host") };
            yield return new object[] { "host", false };
            // Focus on different spacings
            yield return new object[] { "host  test-host", true, GetTuple(Criteria.Host, "  ", "test-host") };
            yield return new object[] { "host	test-host", true, GetTuple(Criteria.Host, "	", "test-host") };
            yield return new object[] { "host test-host  ", true, GetTuple(Criteria.Host, " ", "test-host", "  ") };
            yield return new object[] { "host test-host	", true, GetTuple(Criteria.Host, " ", "test-host", "	") };
        }
        
        [Theory]
        [MemberData(nameof(GetMatchArgumentCriteriaData))]
        public void Parse_MatchArguments(string argument, bool success,
            (Criteria, string, string, string) expected = default)
        {
            var res = MatchStringParser.Parse(argument);
            
            res.IsSuccess.Should().Be(success);
            if (res.IsFailed) return;
            var list = res.Value;
            list.Should().HaveCount(1);
            list.Should().HaveElementAt(0, expected);
        }
        
        public static IEnumerable<object[]> GetMatchMultipleCriteriaData()
        {
            // Focus on different criteria combinations
            yield return new object[] { "host host0 host host1", true, 
                GetTuple(Criteria.Host, " ", "host0", " "),
                GetTuple(Criteria.Host, " ", "host1")
            };
            yield return new object[] { "host host0 user user1", true, 
                GetTuple(Criteria.Host, " ", "host0", " "),
                GetTuple(Criteria.User, " ", "user1")
            };
            yield return new object[] { "host host0,user user1", true, 
                GetTuple(Criteria.Host, " ", "host0", ","),
                GetTuple(Criteria.User, " ", "user1")
            };
            yield return new object[] {"final all", true, 
                GetTuple(Criteria.Final, spacingBack:" "),
                GetTuple(Criteria.All) 
            };
            yield return new object[] {"canonical host0 host host1", false};
            // TODO yield return new object[] {"all host host1", false};
        }

        [Theory]
        [MemberData(nameof(GetMatchMultipleCriteriaData))]
        public void Parse_MatchMultiples(string argument, bool success,
            (Criteria, string, string, string) expected0 = default,
            (Criteria, string, string, string) expected1 = default)
        {
            var res = MatchStringParser.Parse(argument);
            
            res.IsSuccess.Should().Be(success);
            if (res.IsFailed) return;
            var list = res.Value;

            list.Should().HaveCount(2);
            list.Should().HaveElementAt(0, expected0);
            list.Should().HaveElementAt(1, expected1);
        }
    }
}