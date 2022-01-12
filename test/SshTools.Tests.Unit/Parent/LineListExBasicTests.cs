using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Line.Parameter;
using SshTools.Line.Parameter.Keyword;
using SshTools.Parent;
using SshTools.Parent.Host;
using Xunit;
using static SshTools.Tests.Unit.ConfigResources;

namespace SshTools.Tests.Unit.Parent
{
    public class LineListExBasicTests
    {
        [Fact]
        public void Has_TestHasNoUser()
        {
            var config = new SshConfig();
            
            var res = config.Has(Keyword.User);

            res.Should().BeFalse();
        }
        
        [Fact]
        public void Has_TestHasUser()
        {
            const string testUser = "test_user";
            var config = new SshConfig
            {
                User = testUser
            };
            
            var res = config.Has(Keyword.User);

            res.Should().BeTrue();
        }
        
        [Fact]
        public void Get_TestGettingUser()
        {
            const string testUser = "test_user";
            var config = new SshConfig
            {
                User = testUser
            };

            var res = config.Get(Keyword.User);

            res.Should().Be(testUser);
        }

        [Theory]
        [InlineData(ConfigWithUserAtTheStart, 0)]
        [InlineData(ConfigWithEveryParameter, 3)]
        [InlineData(ConfigWithoutAnything, -1)]
        [InlineData(ConfigWithOneNode, -1)]
        public void IndexOf_TestUserInDifferentConfigs(string configString, int expectedIndex)
        {
            var config = DeserializeString(configString);

            var index = config.IndexOf(Keyword.User);

            index.Should().Be(expectedIndex);
        }
        
        [Theory]
        [InlineData(0, true, 0)]
        [InlineData(1, true, 1)]
        [InlineData(2, false)]
        [InlineData(-1, true, 1)]
        [InlineData(-2, true, 0)]
        [InlineData(-3, false)]
        public void Insert_TestAtDifferentPositions(int index, bool expectedResult, int expectedAtIndex = 0)
        {
            const ushort portValue = 123;
            var config = new SshConfig
            {
                User = "user_at_first_position"
            };

            var res = config.Insert(index, Keyword.Port, portValue);
            
            res.IsSuccess.Should().Be(expectedResult);
            if (res.IsFailed)
                return;
            // SelectArg only working as we don't have any comments in this case
            config.SelectArg().Should().HaveElementAt(expectedAtIndex, portValue);
        }
        
        [Theory]
        [InlineData(true, true, 2, "user2")]
        [InlineData(false, false, 1, "user1")]
        public void Insert_TestInsertMultipleUsers(bool ignoreCount,
            bool expectedSecondResult, int expectedConfigCount, string expectedUserName)
        {
            const string userValue = "user1";
            const string user2Value = "user2";
            var config = new SshConfig();

            var res = config.Insert(0, Keyword.User, userValue, ignoreCount);
            var res2 = config.Insert(0, Keyword.User, user2Value, ignoreCount);
            
            res.Should().BeSuccess();
            res2.IsSuccess.Should().Be(expectedSecondResult);
            config.Should().HaveCount(expectedConfigCount);
            config.User.Should().Be(expectedUserName);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Insert_TestInsertMultipleIdentityFiles(bool ignoreCount)
        {
            const string identityFile = "file1";
            const string identityFile2 = "file2";
            const int expectedCount = 2;
            var config = new SshConfig();

            var res = config.Insert(0, Keyword.IdentityFile, identityFile, ignoreCount);
            var res2 = config.Insert(0, Keyword.IdentityFile, identityFile2, ignoreCount);
            
            res.Should().BeSuccess();
            res2.Should().BeSuccess();
            config.Should().HaveCount(expectedCount);
        }
        
        [Fact]
        public void Set_TestBasic()
        {
            const string userValue = "user1";
            var config = new SshConfig();

            var res = config.Set(Keyword.User, userValue);
            
            res.Should().BeSuccess();
            config.Should().HaveCount(1);
            config.User.Should().Be(userValue);
        }
        
        [Fact]
        public void Set_TestSetUserWhenAlreadyExistingUserPresent()
        {
            const string userValue = "user1";
            var config = new SshConfig
            {
                User = "user"
            };

            var res = config.Set(Keyword.User, userValue);
            
            res.Should().BeSuccess();
            config.Should().HaveCount(1);
            config.User.Should().Be(userValue);
        }
        
        [Fact]
        public void Remove_TestBasic()
        {
            var config = new SshConfig
            {
                User = "user"
            };

            var count = config.Remove(Keyword.User);
            
            count.Should().Be(1);
            config.Should().HaveCount(0);
        }
        
        [Fact]
        public void Remove_TestBasicWhenNothingPresent()
        {
            var config = new SshConfig();

            var count = config.Remove(Keyword.User);
            
            count.Should().Be(0);
            config.Should().HaveCount(0);
        }
        
        [Theory]
        [InlineData(1, 1, 5)]
        [InlineData(2, 2, 4)]
        [InlineData(4, 4, 2)]
        [InlineData(10, 4, 2)]
        [InlineData(-1, 0, 6)]
        public void Remove_TestRemovingNItems(int itemsToRemove, int expectedRemovedItems, int expectedItemsLeft)
        {
            var config = new SshConfig();
            config.Insert(-1, Keyword.IdentityFile, "identity1");
            config.Insert(-1, Keyword.IdentityFile, "identity2");
            config.Insert(-1, Keyword.User, "user");
            config.Insert(-1, Keyword.IdentityFile, "identity3");
            config.Insert(-1, Keyword.HostName, "host");
            config.Insert(-1, Keyword.IdentityFile, "identity4");
            
            var count = config.Remove(Keyword.IdentityFile, itemsToRemove);
            
            count.Should().Be(expectedRemovedItems);
            config.Should().HaveCount(expectedItemsLeft);
        }
        
        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 2, 1)]
        [InlineData(10, 2, 1)]
        [InlineData(-1, 0, 3)]
        public void Remove_TestRemovingNItemsOfTypeHost(int itemsToRemove,
            int expectedRemovedItems, int expectedItemsLeft)
        {
            var config = DeserializeString(ConfigWithTwoNodesAndParameterAtBeginning);
            
            var count = config.Remove<HostNode>(_ => true, itemsToRemove);

            count.Should().Be(expectedRemovedItems);
            config.Should().HaveCount(expectedItemsLeft);
            
        }
    }
}