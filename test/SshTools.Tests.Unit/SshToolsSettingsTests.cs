using System.Collections.Generic;
using FluentAssertions;
using SshTools.Parent.Match.Token;
using SshTools.Settings;
using Xunit;

namespace SshTools.Tests.Unit
{
    public class SshToolsSettingsTests
    {
        [Theory]
        [InlineData('k', true)]
        [InlineData('x', false)]
        public void HasToken(char token, bool expected)
        {
            var settings = new SshToolsSettings();
            settings.Add(Token.Values);
            
            var res = settings.Has<Token>(token);
            
            res.Should().Be(expected);
        }
        
        [Theory]
        [InlineData('x')]
        public void GetToken_Unknown(char token)
        {
            var settings = new SshToolsSettings();
            settings.Add(Token.Values);
            
            settings.Invoking(t => t.Get<Token>(token))
                .Should().Throw<KeyNotFoundException>();
        }
        
        [Theory]
        [InlineData('k')]
        public void GetToken_Known(char token)
        {
            var settings = new SshToolsSettings();
            settings.Add(Token.Values);
            
            var res = settings.Get<Token>(token);
            
            res.Should().BeOfType<Token>();
        }
    }
}