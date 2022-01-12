using System.Collections.Generic;
using FluentAssertions;
using SshTools.Parent.Match;
using SshTools.Parent.Match.Token;
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
            
            var res = settings.HasToken(token);
            
            res.Should().Be(expected);
        }
        
        [Theory]
        [InlineData('x')]
        public void GetToken_Unknown(char token)
        {
            var settings = new SshToolsSettings();
            
            settings.Invoking(t => t.GetToken(token))
                .Should().Throw<KeyNotFoundException>();
        }
        
        [Theory]
        [InlineData('k')]
        public void GetToken_Known(char token)
        {
            var settings = new SshToolsSettings();
            
            var res = settings.GetToken(token);
            
            res.Should().BeOfType<Token>();
        }
    }
}