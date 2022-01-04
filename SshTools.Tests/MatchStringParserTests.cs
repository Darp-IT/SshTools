using SshTools.Config.Matching;
using SshTools.Config.Parser;
using Xunit;

namespace SshTools.Tests
{
    public class MatchStringParserTests
    {
        [Fact]
        public void TestMatchingBasic()
        {
            var res = MatchStringParser.Parse("all");
            res.IsSuccess.ShouldBeTrue();
            var list = res.Value;
            
            var (criteria, spacing, value, spacingBack) = list[0];
            criteria.ShouldEqual(Criteria.All);
            spacing.ShouldBeNull();
            value.ShouldBeNull();
            spacingBack.ShouldBeNull();
        }
        
        [Fact]
        public void TestMatchingTwo()
        {
            var res = MatchStringParser.Parse("host test user  user");
            res.IsSuccess.ShouldBeTrue();
            var list = res.Value;
            
            var (criteria, spacing, value, spacingBack) = list[0];
            criteria.ShouldEqual(Criteria.Host);
            spacing.ShouldEqual(" ");
            value.ShouldEqual("test");
            spacingBack.ShouldEqual(" ");
            var (criteria1, spacing1, value1, spacingBack1) = list[1];
            criteria1.ShouldEqual(Criteria.User);
            spacing1.ShouldEqual("  ");
            value1.ShouldEqual("user");
            spacingBack1.ShouldBeNull();
        }
    }
}