using System;
using SshTools.Config.Matching;
using Xunit;

namespace SshTools.Tests
{
    public class TokenParserTests
    {
        private static MatchingContext GetContext() =>
            new("orig_host")
            {
                User = "user",
                Port = 1234,
                HostName = "host",
                HostKeyAlias = "host_alias"
            };

        private static string GetString(string unixOption, string windowsOption) =>
            Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX
                ? unixOption
                : windowsOption;
        private static string GetEnvVariable(string unixOption, string windowsOption) =>
            Environment.GetEnvironmentVariable(GetString(unixOption, windowsOption));

        private static string GetEnvString(string unixOption, string windowsOption) =>
            "{" + GetString(unixOption, windowsOption) + "}";
        
        [Fact]
        public void TestPercentsBasic()
        {
            var context = GetContext();
            context.Expand(Token.OriginalHostName.ToString()).ShouldEqual(context.OriginalHostName);
        }
        
        [Fact]
        public void TestPercentCustomToken()
        {
            var context = GetContext();
            var dummyReplacementToken = new Token('x', _ => "hi");
            SshTools.Configure(config => config.AddTokens(dummyReplacementToken));
            context.Expand("%x").ShouldEqual("hi");
            SshTools.Configure(config => config.SetTokens(Token.Values));
        }
        
        [Fact]
        public void TestPercentUnknownToken()
        {
            var context = GetContext();
            context.Expand("%x").IsFailed.ShouldBeTrue();
        }
        
        [Fact]
        public void TestPercentDoublePercent()
        {
            var context = GetContext();
            context.Expand("%%").ShouldEqual("%");
        }
        
        [Fact]
        public void TestPercentHomeDir()
        {
            var context = GetContext();
            context.Expand("%d/.ssh").ShouldEqual($"{GetEnvVariable("HOME", "UserProfile")}/.ssh");
        }
        
        [Fact]
        public void TestTildeHomeDir()
        {
            var context = GetContext();
            context.Expand("~/.ssh").ShouldEqual($"{GetEnvVariable("HOME", "UserProfile")}/.ssh");
        }
        
        [Fact]
        public void TestPercentRandom()
        {
            var context = GetContext();
            context.Expand("asd%p%%sd ss %n  asd&sd %%%%asd%%%%%k")
                .ShouldEqual($"asd{context.Port}%sd ss {context.OriginalHostName}  asd&sd %%asd%%{context.HostKeyAlias}");
        }
        
        [Fact]
        public void TestEnvironmentVariablesBasic()
        {
            var context = GetContext();
            context.Expand($"${GetEnvString("HOME", "UserProfile")}/.ssh")
                    .ShouldEqual($"{GetEnvVariable("HOME", "UserProfile")}/.ssh");
        }
        
        [Fact]
        public void TestEnvironmentVariablesEmpty()
        {
            var context = GetContext();
            context.Expand("${}")
                .ShouldEqual("${}");
        }
        
        [Fact]
        public void TestEnvironmentVariablesRandom()
        {
            var context = GetContext();
            context.Expand($"test${GetEnvString("HOME", "UserProfile")} as {{ssss}} " +
                           $"$${GetEnvString("HOME", "UserProfile")}$asd$a{{sss}}$ {{asd}}sss${{}}")
                    .ShouldEqual($"test{GetEnvVariable("HOME", "UserProfile")} as {{ssss}} " +
                                 $"${GetEnvVariable("HOME", "UserProfile")}$asd$a{{sss}}$ {{asd}}sss${{}}");
        }
    }
}