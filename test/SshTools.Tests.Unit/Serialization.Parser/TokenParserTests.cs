using System;
using System.Collections.Generic;
using FluentResults.Extensions.FluentAssertions;
using SshTools.Parent.Match;
using SshTools.Parent.Match.Token;
using SshTools.Serialization.Parser;
using Xunit;

namespace SshTools.Tests.Unit.Serialization.Parser
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

        [Fact]
        public void TestPercentsBasic()
        {
            var context = GetContext();
            
            var res = context.Expand(Token.OriginalHostName.ToString());
            
            res.Should().HaveValue(context.OriginalHostName);
        }

        [Fact]
        public void TestPercentCustomToken()
        {
            const char exampleKey = 'x';
            const string exampleValue = "hi";
            var context = GetContext();
            var dummyReplacementToken = new Token(exampleKey, _ => exampleValue);
            SshTools.Configure(config => config.AddTokens(dummyReplacementToken));
            
            var res = context.Expand("%" + exampleKey);
            
            res.Should().HaveValue(exampleValue);
            SshTools.Configure(config => config.SetTokens(Token.Values));
        }
        
        [Fact]
        public void TestPercentUnknownToken()
        {
            const char unknownKey = 'x';
            var context = GetContext();
            
            var res = context.Expand("%" + unknownKey);
            
            res.Should().BeFailure();
        }

        private static Func<MatchingContext, string> GetFunc(Func<MatchingContext, string> func) => func;

        public static IEnumerable<object[]> GetPercentTests()
        {
            // todos should be moved to integration tests or something like that
            
            // Focus on basic functionality
            yield return new object[] { Token.HostKeyAlias.ToString(), GetFunc(context => context.HostKeyAlias) };
            // TODO yield return new object[] { Token.LocalHomeDirectory.ToString(), GetFunc(_ => GetEnvVariable("HOME", "UserProfile")) };
            // TODO yield return new object[] { Token.LocalHostName.ToString(), GetFunc(context => ...) };
            yield return new object[] { Token.LocalUserName.ToString(), GetFunc(context => context.LocalUser) };
            // TODO yield return new object[] { Token.LocalHostNameComplete.ToString(), GetFunc(_ => Dns.GetHostName()) };
            // TODO yield return new object[] { Token.LocalHostAndDomainName.ToString(), GetFunc(_ => IPGlobalProperties.GetIPGlobalProperties().DomainName) };
            yield return new object[] { Token.OriginalHostName.ToString(), GetFunc(context => context.OriginalHostName) };
            yield return new object[] { Token.Percent.ToString(), GetFunc(_ => "%") };
            yield return new object[] { Token.RemoteHostName.ToString(), GetFunc(context => context.HostName) };
            yield return new object[] { Token.RemotePort.ToString(), GetFunc(context => context.Port.ToString()) };
            yield return new object[] { Token.UserName.ToString(), GetFunc(context => context.User) };
            
            // Focus on multiple, longer strings
            yield return new object[]
            {
                "asd%p%%sd ss %n  asd&sd %%%%asd%%%%%k",
                GetFunc(context => $"asd{context.Port}%sd ss {context.OriginalHostName}  asd&sd %%asd%%{context.HostKeyAlias}")
            };
        }
        
        [Theory]
        [MemberData(nameof(GetPercentTests))]
        public void Parse_TestRandom(string tokenString, Func<MatchingContext, string> expected)
        {
            var context = GetContext();
            
            var res = TokenParser.Parse(tokenString, context);
            
            res.Should().HaveValue(expected(context));
        }
        
        // TODO make stuff integration or mock
        
        private static string GetString(string unixOption, string windowsOption) =>
            Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX
                ? unixOption
                : windowsOption;
        private static string GetEnvVariable(string unixOption, string windowsOption) =>
            Environment.GetEnvironmentVariable(GetString(unixOption, windowsOption));

        private static string GetEnvString(string unixOption, string windowsOption) =>
            "{" + GetString(unixOption, windowsOption) + "}";
        
        [Fact]
        public void TestPercentHomeDir()
        {
            var context = GetContext();
            
            var res = context.Expand("%d/.ssh");
            
            res.Should().HaveValue($"{GetEnvVariable("HOME", "UserProfile")}/.ssh");
        }
        
        [Fact]
        public void TestTildeHomeDir()
        {
            var context = GetContext();
            
            var res = context.Expand("~/.ssh");
            
            res.Should().HaveValue($"{GetEnvVariable("HOME", "UserProfile")}/.ssh");
        }
        
        [Fact]
        public void TestEnvironmentVariablesBasic()
        {
            var context = GetContext();
            
            var res =context.Expand($"${GetEnvString("HOME", "UserProfile")}/.ssh");
            
            res.Should().HaveValue($"{GetEnvVariable("HOME", "UserProfile")}/.ssh");
            
        }
        
        [Fact]
        public void TestEnvironmentVariablesEmpty()
        {
            var context = GetContext();
            context.Expand("${}")
                .Should().HaveValue("${}");
        }
        
        [Fact]
        public void TestEnvironmentVariablesRandom()
        {
            var context = GetContext();
            context.Expand($"test${GetEnvString("HOME", "UserProfile")} as {{ssss}} " +
                           $"$${GetEnvString("HOME", "UserProfile")}$asd$a{{sss}}$ {{asd}}sss${{}}")
                    .Should().HaveValue($"test{GetEnvVariable("HOME", "UserProfile")} as {{ssss}} " +
                                 $"${GetEnvVariable("HOME", "UserProfile")}$asd$a{{sss}}$ {{asd}}sss${{}}");
        }
    }
}