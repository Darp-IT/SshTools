using System;
using System.Net;
using System.Net.NetworkInformation;
using SshTools.Settings;
using SshTools.Util;

namespace SshTools.Parent.Match.Token
{
    public partial class Token
    {
        /// <summary>
        /// <br/> Default return: 'NONE'
        /// </summary>
        private const string DefaultReturn = "NONE";
        
        //-----------------------------------------------------------------------//
        //                          Implemented Tokens
        //-----------------------------------------------------------------------//

        /// %: A literal ‘%’
        public static readonly Token Percent = new Token('%', context => "%");

        /// d: Local user's home directory
        public static readonly Token LocalHomeDirectory = new Token('d', context =>
            Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.GetEnvironmentVariable("UserProfile"));

        /// h: The remote hostname
        public static readonly Token RemoteHostName = new Token('h', context => context.HostName);

        /// <summary>
        /// l: A string describing the reason for a KnownHostsCommand execution, that is containing a
        /// <list type="bullet">
        /// <item>ADDRESS when looking up a host by address (only when CheckHostIP is enabled)</item>
        /// <item>HOSTNAME when searching by hostname</item>
        /// <item>ORDER when preparing the host key algorithm preference list to use for the destination host</item>
        /// </list>
        /// </summary>
        public static readonly Token LocalHostNameComplete = new Token('l', context => Dns.GetHostName());

        /// k: The host key alias if specified, otherwise the original remote hostname given on the command line
        public static readonly Token HostKeyAlias = new Token('k', context => context.HostKeyAlias ?? context.HostName);

        /// L: The local hostname
        public static readonly Token LocalHostName = new Token('L', context =>
            Dns.GetHostName().Substring(0, SshToolsSettings.MaxHostCharacters).ReplaceFirst(".", ""));

        /// I: The local hostname, including the domain name
        public static readonly Token LocalHostAndDomainName =
            new Token('I', context => IPGlobalProperties.GetIPGlobalProperties().DomainName);

        /// n: The original remote hostname, as given on the command line
        public static readonly Token OriginalHostName = new Token('n', context => context.OriginalHostName);

        /// p: The remote port
        public static readonly Token RemotePort = new Token('p', context => context.Port.ToString());

        /// r: The local username
        public static readonly Token UserName = new Token('r', context => context.User);

        /// u: The local username
        public static readonly Token LocalUserName = new Token('u', context => context.LocalUser);
        
        //-----------------------------------------------------------------------//
        //                          Unimplemented Tokens
        //-----------------------------------------------------------------------//

        /// H: The known_hosts hostname or address that is being searched for
        [Obsolete("Unknown behavior", true)]
        public static readonly Token HostFileHostName = new Token('H', context => throw new NotImplementedException());

        /// i: The local user ID
        [Obsolete("Way needed to get uid (or sid on windows?)", true)]
        public static readonly Token UserId = new Token('i', context => throw new NotImplementedException());

        /// <summary>
        /// C: Hash of %l%h%p%r
        /// <list type="bullet">
        /// <item><see cref="Token.LocalHostNameComplete"/>(l)</item>
        /// <item><see cref="Token.RemoteHostName"/>(h)</item>
        /// <item><see cref="Token.RemotePort"/>(p)</item>
        /// <item><see cref="Token.UserName"/>(r)</item>
        /// </list>
        /// </summary>
        [Obsolete("HashFunction needed", true)]
        public static readonly Token HashConnection = new Token('C', context => DefaultReturn);

        ///<summary>
        /// f: The fingerprint of the server's host key
        /// <inheritdoc cref="DefaultReturn"/>
        /// </summary>
        [Obsolete("Fingerprint function needed", true)]
        public static readonly Token ServerKeyFingerprint = new Token('f', context => DefaultReturn);

        ///<summary>
        /// t: The type of the server host key, e.g. ssh-ed25519
        /// <inheritdoc cref="DefaultReturn"/>
        /// </summary>
        [Obsolete("Fingerprint function needed", true)]
        public static readonly Token HostKeyType = new Token('t', context => DefaultReturn);

        ///<summary>
        /// T: The local tun or tap network interface assigned if tunnel forwarding was requested
        /// <inheritdoc cref="DefaultReturn"/>
        /// </summary>
        [Obsolete("Unknown behavior", true)]
        public static readonly Token TunnelNetworkInterface = new Token('T', context => DefaultReturn);

        ///<summary>
        /// K: The base64 encoded host key
        /// <inheritdoc cref="DefaultReturn"/>
        /// </summary>
        [Obsolete("Base64 encoding function needed", true)]
        public static readonly Token HostKey = new Token('K', context => DefaultReturn);
    }
}