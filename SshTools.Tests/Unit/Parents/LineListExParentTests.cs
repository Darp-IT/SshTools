using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using Xunit;

using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parents
{
    public class LineListExParentTests
    {
        [Theory]
        [InlineData("host1", false, true)]
        [InlineData("host", false, false)]
        [InlineData("host3", false, false)]
        [InlineData("host123123", false, false)]
        
        [InlineData("host1", true, true)]
        [InlineData("host3", true, true)]
        [InlineData("host123123", true, true)]
        [InlineData("host", true, false)]
        public void Has_TestHasNoHost(string hostName, bool matching, bool expectedHasConfig)
        {
            var config = DeserializeString(ConfigWithNodesWithPatterns);

            var option = matching ? MatchingOptions.MATCHING : MatchingOptions.EXACT;
            var hasConfig = config.Has(hostName, option);

            hasConfig.Should().Be(expectedHasConfig);
        }
        
        [Theory]
        [InlineData("host", -1)]
        [InlineData("host1", 1)]
        [InlineData("host2", 2)]
        public void IndexOf_TestDifferentHostNames(string hostName, int expectedIndex)
        {
            var config = DeserializeString(ConfigWithTwoNodesAndParameterAtBeginning);

            var index = config.IndexOf(hostName);

            index.Should().Be(expectedIndex);
        }
        
        [Theory]
        [InlineData("host", false)]
        [InlineData("host1", true)]
        [InlineData("host2", true)]
        public void Get_TestDifferentHostNames(string hostName, bool expectedHostEntry)
        {
            var config = DeserializeString(ConfigWithTwoNodesAndParameterAtBeginning);

            var host = config.Get(hostName);
            var hasHost = host != null;

            hasHost.Should().Be(expectedHostEntry);
            if (!hasHost) return;
            host.Name.Should().Be(hostName);
        }
        
        [Theory]
        [InlineData("host", true)]
        [InlineData("", false)]
        [InlineData("       ", false)]
        [InlineData(null, false)]
        public void InsertHost_TestBasics(string hostName, bool expectedCouldSet)
        {
            var config = new SshConfig();

            var res = config.InsertHost(0, hostName);

            res.IsSuccess.Should().Be(expectedCouldSet);
            config.Should().HaveCount(expectedCouldSet ? 1 : 0);
            if (!expectedCouldSet) return;
            res.Value.Should().BeOfType<HostNode>();
        }
        
        [Theory]
        [InlineData(0, true, 0)]
        [InlineData(1, true, 1)]
        [InlineData(2, false)]
        [InlineData(-1, true, 1)]
        [InlineData(-2, true, 0)]
        [InlineData(-3, false)]
        public void InsertHost_TestAtDifferentPositions(int index, bool expectedSuccess, int expectedAtIndex = 0)
        {
            const string hostName = "host";
            var config = new SshConfig
            {
                User = "user_at_first_position"
            };

            var res = config.InsertHost(index, hostName);
            
            res.IsSuccess.Should().Be(expectedSuccess);
            if (res.IsFailed)
                return;
            // SelectArg only working as we don't have any comments in this case
            config.SelectArg().Should().HaveElementAt(expectedAtIndex, res.Value);
        }
        
        public static IEnumerable<object[]> GetSingleCriteria()
        {
            yield return new object[]{ Criteria.All, true };
            yield return new object[]{ Criteria.Canonical, true };
            yield return new object[]{ Criteria.Final, true };
            yield return new object[]{ null, false };
        }
        
        [Theory]
        [MemberData(nameof(GetSingleCriteria))]
        public void InsertMatch_TestBasicSingles(Criteria criteria, bool expectedCouldSet)
        {
            var config = new SshConfig();

            var res = config.InsertMatch(0, criteria);

            res.IsSuccess.Should().Be(expectedCouldSet);
            config.Should().HaveCount(expectedCouldSet ? 1 : 0);
            if (!expectedCouldSet) return;
            res.Value.Should().BeOfType<MatchNode>();
        }
        
        public static IEnumerable<object[]> GetArgumentCriteria()
        {
            yield return new object[] { true, Criteria.Host, "host"};
            yield return new object[] { true, Criteria.User, "user" };
            yield return new object[] { true, Criteria.LocalUser, "local_user" };
            yield return new object[] { true, Criteria.OriginalHost, "original_host" };
            yield return new object[] { true, Criteria.Host, "host1", "!host1", "host3"};
            yield return new object[] { false, Criteria.Host, "", "", "" };
            yield return new object[] { false, Criteria.Host, "" };
            yield return new object[] { false, Criteria.Host, "       " };
            yield return new object[] { false, Criteria.Host, null };
            yield return new object[] { false, null, "asd"};
        }
        
        [Theory]
        [MemberData(nameof(GetArgumentCriteria))]
        public void InsertMatch_TestBasicArguments(bool expectedCouldSet, ArgumentCriteria criteria, params string[] argument)
        {
            var config = new SshConfig();

            var res = config.InsertMatch(0, criteria, argument);

            res.IsSuccess.Should().Be(expectedCouldSet);
            config.Should().HaveCount(expectedCouldSet ? 1 : 0);
            if (!expectedCouldSet) return;
            res.Value.Should().BeOfType<MatchNode>();
        }
        
        [Theory]
        [InlineData(0, true, 0)]
        [InlineData(1, true, 1)]
        [InlineData(2, false)]
        [InlineData(-1, true, 1)]
        [InlineData(-2, true, 0)]
        [InlineData(-3, false)]
        public void InsertMatch_TestAtDifferentPositions(int index, bool expectedSuccess, int expectedAtIndex = 0)
        {
            var config = new SshConfig
            {
                User = "user_at_first_position"
            };

            var res = config.InsertMatch(index, Criteria.All);
            
            res.IsSuccess.Should().Be(expectedSuccess);
            if (res.IsFailed)
                return;
            // SelectArg only working as we don't have any comments in this case
            config.SelectArg().Should().HaveElementAt(expectedAtIndex, res.Value);
        }
        
        [Fact]
        public void SetHost_TestBasic()
        {
            const string hostName = "hostName";
            var config = new SshConfig();

            var res = config.SetHost(hostName);
            
            res.Should().BeSuccess();
            config.Should().HaveCount(1);
            res.Value.Should().BeOfType<HostNode>();
            config[hostName].Name.Should().Be(hostName);
        }
        
        [Fact]
        public void SetHost_TestSetUserWhenAlreadyExistingUserPresent()
        {
            const string hostName = "hostName";
            const string userName = "userName";
            var config = new SshConfig();
            config.InsertHost(0, hostName);
            config.Get(hostName).User = userName;
            
            var res = config.SetHost(hostName);
            var host = config.Get(hostName);

            res.Should().BeSuccess();
            config.Should().HaveCount(1);
            host.Should().BeSameAs(res.Value);
            host.Should().BeOfType<HostNode>();
            host.Has(Keyword.User).Should().BeFalse();
        }
        
        [Fact]
        public void SetMatch_TestBasicSingle()
        {
            var config = new SshConfig();
            var criteria = Criteria.All;

            var res = config.SetMatch(criteria);
            
            res.Should().BeSuccess();
            config.Should().HaveCount(1);
            config.Has(criteria.ToString()).Should().BeTrue();
        }
        
        [Fact]
        public void SetMatch_TestBasicArgument()
        {
            const string argument = "host";
            var criteria = Criteria.Host;
            var config = new SshConfig();

            var res = config.SetMatch(criteria, argument);
            
            res.Should().BeSuccess();
            config.Should().HaveCount(1);
            config.Has(criteria + " " + argument).Should().BeTrue();
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, 0)]
        [InlineData(ConfigWithOnlyANode, 1)]
        [InlineData(ConfigWithEveryParameter, 2)]
        public void Nodes_TestCount(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var res = config.Nodes();

            res.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, 0)]
        [InlineData(ConfigWithOnlyANode, 1)]
        [InlineData(ConfigWithNodesWithPatterns, 4)]
        public void Hosts_TestCount(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var res = config.Hosts();

            res.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(ConfigWithoutAnything, 0)]
        [InlineData(ConfigWithEveryParameter, 1)]
        public void Matches_TestCount(string configString, int expectedCount)
        {
            var config = DeserializeString(configString);

            var res = config.Matches();

            res.Should().HaveCount(expectedCount);
        }
    }
}