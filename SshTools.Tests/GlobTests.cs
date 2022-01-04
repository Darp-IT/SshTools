using Xunit;
using static SshTools.Config.Parser.Globber;

namespace SshTools.Tests
{
    public class GlobTests
    {
        [Fact]
        public void glob_asterisk_mark()
        {
            Glob("*", "laputa").ShouldEqual(true);
            Glob("lap*", "laputa").ShouldEqual(true);
            Glob("lap*ta", "laputa").ShouldEqual(true);
            Glob("laputa*", "laputa").ShouldEqual(true);

            Glob("lap*", "castle").ShouldEqual(false);
        }

        [Fact]
        public void glob_question_mark()
        {
            Glob("lap?ta", "laputa").ShouldEqual(true);
            Glob("lap?ta", "lap.ta").ShouldEqual(true);
            Glob("laputa?", "laputa1").ShouldEqual(true);
            Glob("laputa?", "laputa").ShouldEqual(false);
            Glob("lap?ta", "lapta").ShouldEqual(false);
            Glob("lap?ta", "lapuuta").ShouldEqual(false);
            Glob("lap?ta", "castle").ShouldEqual(false);
        }

        [Fact]
        public void glob_pattern_list()
        {
            Glob("laputa,castle", "laputa").ShouldEqual(true);
            Glob("castle,in,the,sky", "laputa").ShouldEqual(false);
        }

        [Fact]
        public void glob_negated_pattern_list()
        {
            Glob("!*.dialup.example.com,*.example.com", "www.example.com").ShouldEqual(true);
            Glob("!*.dialup.example.com,*.example.com", "www.dialup.example.com").ShouldEqual(false);
            Glob("*.example.com,!*.dialup.example.com", "www.dialup.example.com").ShouldEqual(false);
        }

        [Fact]
        public void glob_the_whole_string()
        {
            Glob("example", "example1").ShouldEqual(false);
            Glob("* !test", "abc").ShouldEqual(true);
            Glob("* !test", "test").ShouldEqual(false);
        }
        
        [Fact]
        public void glob_empty()
        {
            Glob("example", "").ShouldEqual(true);
            Glob("* !test", "").ShouldEqual(true);
        }
    }
}