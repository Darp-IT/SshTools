using SshTools.Config.Parser;
using Xunit;

namespace SshTools.Tests
{
    public class LineParserTests
    {
        [Fact]
        public void TestIsComment()
        {
            LineParser.IsConfigComment("#asdasd").ShouldBeTrue();
            LineParser.IsConfigComment("#	asdasd").ShouldBeTrue();
            LineParser.IsConfigComment("	#asdasd").ShouldBeTrue();
            LineParser.IsConfigComment("	  #		# asdasd").ShouldBeTrue();
            LineParser.IsConfigComment("### #asdasd").ShouldBeTrue();
        }

        private static void TestTrimFront_(string line, string expected, string expLine)
        {
            var line2 = LineParser.TrimFront(line, out var res);
            line2.ShouldEqual(expLine);
            res.ShouldEqual(expected);
        }
        
        [Fact]
        public void TestTrimFront()
        {
            TestTrimFront_("Host hostName", "", "Host hostName");
            TestTrimFront_("  host=hostName", "  ", "host=hostName");
            TestTrimFront_("	Host	hostName", "	", "Host	hostName");
            TestTrimFront_("  	 HOST  =  hostName", "  	 ", "HOST  =  hostName");
        }

        private static void TrimKey_(string line, bool success, string expected, string expLine)
        {
            var line2 = LineParser.TrimKey(line, out var res);
            res.IsSuccess.ShouldEqual(success);
            if (!success) return;
            
            line2.ShouldEqual(expLine);
            res.Value.ShouldEqual(expected);
        }
        
        [Fact]
        public void TestTrimKey()
        {
            TrimKey_("Host hostName", true, "Host", " hostName");
            TrimKey_("host=hostName",true, "host", "=hostName");
            TrimKey_("HOST  =  hostName", true,"HOST", "  =  hostName");
            TrimKey_("	Host	hostName", false,"", "	hostName");
        }
        
        private static void TrimSeparator_(string line, bool success, string expected, string expLine)
        {
            var line2 = LineParser.TrimSeparator(line, out var res);
            res.IsSuccess.ShouldEqual(success);
            
            if (!success) return;
            line2.ShouldEqual(expLine);
            res.Value.ShouldEqual(expected);
        }
        
        [Fact]
        public void TestTrimSeparator()
        {
            TrimSeparator_(" hostName", true, " ", "hostName");
            TrimSeparator_("=hostName",true, "=", "hostName");
            TrimSeparator_("	hostName", true,"	", "hostName");
            TrimSeparator_("  =  \"hostName\"", true,"  =  ", "\"hostName\"");
            TrimSeparator_("  =  host=Name", true,"  =  ", "host=Name");
            TrimSeparator_("hostName", false,"", "");
            TrimSeparator_("  = == hostName", false,"", "");
        }
        
        private static void TrimArgument_(string line, string expected, string expLine, bool isQuoted)
        {
            var line2 = LineParser.TrimArgument(line, out var res, out var quoted);
            res.IsSuccess.ShouldBeTrue();
            line2.ShouldEqual(expLine);
            res.Value.ShouldEqual(expected);
            quoted.ShouldEqual(isQuoted);
        }
        
        [Fact]
        public void TrimArgument()
        {
            TrimArgument_("hostName", "hostName", "", false);
            TrimArgument_("host   =  asdName", "host   =  asdName","", false);
            TrimArgument_("ho  st	Name", "ho  st	Name", "", false);
            TrimArgument_("hostName\"", "hostName\"", "", false); //?
            
            TrimArgument_("\"hostName\"", "hostName","", true); //?
            TrimArgument_("\"\"hostName\"\"", "\"hostName\"","", true); //?
            
            TrimArgument_("hostName    ", "hostName","    ", false);
            TrimArgument_("hostName   	 ", "hostName","   	 ", false);
        }
    }
}   