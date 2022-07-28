using System.Collections.Generic;
using FluentAssertions;
using SshTools.Line.Parameter.Keyword;
using SshTools.Parent.Match.Token;
using SshTools.Serialization.Parser;
using SshTools.Settings;
using Xunit;

namespace SshTools.Tests.Unit
{
    public class SshToolsSettingsTests
    {
        [Theory]
        [InlineData('k', true)]
        [InlineData('x', false)]
        public void Has_TestTokens(char token, bool expected)
        {
            var settings = new SshToolsSettings();
            settings.Add(Token.Values);
            
            var res = settings.Has<Token>(token);
            
            res.Should().Be(expected);
        }
        
        [Theory]
        [InlineData('x')]
        public void Get_TestUnknownTokens(char token)
        {
            var settings = new SshToolsSettings();
            settings.Add(Token.Values);
            
            settings.Invoking(t => t.Get<Token>(token))
                .Should().Throw<KeyNotFoundException>();
        }
        
        [Theory]
        [InlineData('k')]
        public void Get_TestKnownTokens(char token)
        {
            var settings = new SshToolsSettings();
            settings.Add(Token.Values);
            
            var res = settings.Get<Token>(token);
            
            res.Should().BeOfType<Token>();
        }
        
        [Fact]
        public void Add_TestIfKeywordIsContainedAfterAdding()
        {
            const string name = "Test";
            var settings = new SshToolsSettings();
            settings.Add(Keyword.Values);

            settings.Add(new Keyword<string>(name, ArgumentParser.String));
            var key = settings.Get<Keyword>(name);

            key.Name.Should().Be(name);
        }
    }
}