using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Config.Parser;
using Xunit;

namespace SshTools.Tests.Unit.Parser
{
    public class LineParserTests
    {
        [Theory]
        [InlineData("#asd", true)]
        [InlineData("#	asd", true)]
        [InlineData("	#asd", true)]
        [InlineData("	  #		# asd", true)]
        [InlineData("### #asd", true)]
        [InlineData("", true)]
        [InlineData("       ", true)]
        [InlineData("asd", false)]
        [InlineData("              te", false)]
        [InlineData("~", false)]
        public void IsConfigComment_TestLines(string line, bool isComment) => 
            LineParser.IsConfigComment(line).Should().Be(isComment);

        [Theory]
        [InlineData("Host hostName", "", "Host hostName")]
        [InlineData("~~6789Host hostName", "", "~~6789Host hostName")]
        [InlineData("  host=hostName", "  ", "host=hostName")]
        [InlineData("	Host	hostName", "	", "Host	hostName")]
        [InlineData("  	 HOST  =  hostName", "  	 ", "HOST  =  hostName")]
        [InlineData("   ", "   ", "")]
        [InlineData("# asd", "", "# asd")]
        [InlineData("   #   ", "   ", "#   ")]
        public void TrimFront_TestLines(string line, string expected, string expLine)
        {
            var line2 = LineParser.TrimFront(line, out var res);
            
            res.Should().Be(expected);
            line2.Should().Be(expLine);
        }

        [Theory]
        [InlineData("Host hostName", true, "Host", " hostName")]
        [InlineData("host=hostName", true, "host", "=hostName")]
        [InlineData("HOST  =  hostName", true, "HOST", "  =  hostName")]
        [InlineData("	Host	hostName", false, "", "	hostName")]
        public void TrimKey_TestLines(string line, bool success, string expected, string expLine)
        {
            var line2 = LineParser.TrimKey(line, out var res);
            
            res.IsSuccess.Should().Be(success);
            if (!success) return;
            line2.Should().Be(expLine);
            res.Should().HaveValue(expected);
        }
        
        [Theory]
        [InlineData(" hostName", true, " ", "hostName")]
        [InlineData("=hostName",true, "=", "hostName")]
        [InlineData("	hostName", true, "	", "hostName")]
        [InlineData("  =  \"hostName\"", true, "  =  ", "\"hostName\"")]
        [InlineData("  =  host=Name", true, "  =  ", "host=Name")]
        [InlineData("hostName", false, "", "")]
        [InlineData("  = == hostName", false, "", "")]
        public void TrimSeparator_TestLines(string line, bool success, string expected, string expLine)
        {
            var line2 = LineParser.TrimSeparator(line, out var res);
            
            res.IsSuccess.Should().Be(success);
            if (!success) return;
            line2.Should().Be(expLine);
            res.Should().HaveValue(expected);
        }
        
        [Theory]
        [InlineData("hostName", "hostName", "", false)]
        [InlineData("host   =  asdName", "host   =  asdName","", false)]
        [InlineData("ho  st	Name", "ho  st	Name", "", false)]
        // Focus on end of line
        [InlineData("hostName    ", "hostName","    ", false)]
        [InlineData("hostName   	 ", "hostName","   	 ", false)]
        // Focus on quoted
        [InlineData("\"hostName\"", "hostName","", true)]
        [InlineData("\"\"hostName\"\"", "\"hostName\"", "", true)]
        [InlineData("hostName\"", "hostName\"", "", false)]
        [InlineData("\"hostName", "\"hostName", "", false)]
        public void TrimArgument_TestLines(string line, string expected, string expLine, bool isQuoted)
        {
            var line2 = LineParser.TrimArgument(line, out var res, out var quoted);
            
            line2.Should().Be(expLine);
            res.Should().Be(expected);
            quoted.Should().Be(isQuoted);
        }
    }
}   