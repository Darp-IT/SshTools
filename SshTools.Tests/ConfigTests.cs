using System;
using System.IO;
using System.Linq;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using Xunit;

namespace SshTools.Tests
{
    public class ParserTests
    {
        /*/// One should be able to reconstruct the original config file
        Fact]
        public void TestReconstructString()
        {
            var text = File.ReadAllText("configs/config");
            var configRes = SshConfig.DeserializeString(text);
            configRes.IsSuccess.ShouldBeTrue();

            var config = configRes.Value;
            var text2 = config.Serialize();
            text.ShouldEqual(text2);
        }*/

        private static SshConfig LoadConfig(string name, bool compile = false)
        {
            var configRes = SshConfig.ReadFile(name);
            configRes.IsSuccess.ShouldBeTrue();
            return compile
                ? configRes.Value.Compile()
                : configRes.Value;
        }

        [Fact]
        public void TestConfigure()
        {
            SshTools.Configure(settings => settings.SetKeywords(Array.Empty<Keyword>()));
            var configRes = SshConfig.ReadFile("configs/config");
            configRes.IsFailed.ShouldBeTrue();
            SshTools.Configure(settings => settings.SetKeywords(Keyword.Values));
            var configRes2 = SshConfig.ReadFile("configs/config");
            configRes2.IsFailed.ShouldBeFalse();
        }

        /// One should be able to return a version with flattened includes from a node
        [Fact]
        public void TestFlattenHost()
        {
            var config = LoadConfig("configs/config");
            var jumpHost = config
                .Select(p => p.Argument as HostNode)
                .First(host => host is {MatchString: "jumphost"});
            var includedJumpHost = jumpHost
                .Flatten()
                .Cloned()
                .FirstToHost("dummy");

            //Test that there are no more includes anymore
            jumpHost
                .Count(p => p.IsInclude() || p.Argument is Node node && node.Has(Keyword.Include))
                .ShouldEqual(1);
            includedJumpHost
                .Count(p => p.IsInclude())
                .ShouldEqual(0);

            //Verify list is independent of old config
            jumpHost.Get(Keyword.Include).HostName.ShouldEqual("jumphost.com");
            includedJumpHost.HostName.ShouldEqual("jumphost.com");
            includedJumpHost.HostName = "abc.com";
            includedJumpHost.HostName.ShouldEqual("abc.com");
            jumpHost.Get(Keyword.Include).HostName.ShouldEqual("jumphost.com");
        }

        /// One should be able to get a list of all nodes, with includes being stripped
        [Fact]
        public void TestFlattenConfig()
        {
            var config = LoadConfig("configs/config");
            var includedConfig = config
                .Flatten()
                .ToConfig();

            config
                .Count(p => p.IsInclude() || p.Argument is Node node && node.Has(Keyword.Include))
                .ShouldEqual(2);
            includedConfig
                .Count(p => p.IsInclude())
                .ShouldEqual(0);
        }

        [Fact]
        public void TestConfigFind()
        {
            var config = LoadConfig("configs/config");
            // Return exactly the HostNode with name jumphost
            // (choosing compile will return a uncoupled host of the whole config)
            var compiledHost = config.Get("jumphost");
            compiledHost.ShouldBe<HostNode>().Count.ShouldEqual(2);
            
            // Return a list of all parents, that are matching the search
            var allHosts = config.GetAll("dummy");
            allHosts.Count.ShouldEqual(3);
            
            // Return a HostNode with a summary of all matching nodes
            var foundHost = config.Find("jumphost");
            foundHost.ShouldBe<HostNode>().Count.ShouldEqual(3);

            // Return a Match node, that fulfills the match filter
            //config.FindMatch();
        }

        /// One should be able to compile to a host, that has all parameters,
        /// that match the search string (also from includes)
        [Fact]
        public void TestCompileConfig()
        {
            var config = LoadConfig("configs/config");
            config.Count.ShouldEqual(8);
            var host = config.Compile();
            host.Count.ShouldEqual(11);
        }

        /// One should be able to compile to a host, that has all parameters,
        /// that match the search string (also from includes)
        [Fact]
        public void TestCompileHost()
        {
            var config = LoadConfig("configs/config");
            var host = config.Find("test");
            host.Port.ShouldEqual((ushort) 54321);
            host.User.ShouldEqual("testuser");
            host.IdentityFile.ShouldEqual("~/.ssh/id_rsa-cert.pub");
        }

        /// A compiled host should be fully readable by Keyword and indexing or Parameter
        [Fact]
        public void TestReadCompiledHost()
        {
            var config = LoadConfig("configs/config");
            var host = config.Find("eva");
            Assert.NotNull(host);

            host[Keyword.User].ShouldEqual("evauser");
            host.Get(Keyword.User).ShouldEqual("evauser");
            host.User.ShouldEqual("evauser");
            host.WhereParam(Keyword.IdentityFile)
                .SelectArg()
                .First()
                .ShouldEqual("~/.ssh/id_rsa-cert.pub");

            //Host should return a list of compiled parameters
            host.Count.ShouldEqual(5);
        }

        /// A compiled host should be editable (add, change, remove), but this should not affect the original config
        [Fact]
        public void TestEditHost()
        {
            var config = LoadConfig("configs/config");
            var host = config.Get("dummy");

            //Remove something from a compiled host
            Assert.NotNull(host.HostName);
            host.Remove(Keyword.HostName);
            Assert.Null(host.HostName);
            host.Has(Keyword.HostName).ShouldBeFalse();

            //Insert at position to host
            host.Insert(0, Keyword.User, "known1");
            host.WhereParam(Keyword.User).ToList()[0].Argument.ShouldEqual("known1");

            //Add something to the host
            host.Set(Keyword.User, "known2");
            host.User.ShouldEqual("known2");

            //Repeat for different methods
            host.User = "known3";
            host[Keyword.User].ShouldEqual("known3");

            host.Set(Keyword.User, "known4");
            host.User.ShouldEqual("known4");


            //Insert at position to host
            var res = host.Insert(1, Keyword.User, "known5");
            res.IsFailed.ShouldBeTrue();
            host
                .WhereParam(Keyword.User)
                .Count()
                .ShouldEqual(1);

            host.Insert(0, Keyword.IdentityFile, "abc1");
            host.Insert(-1, Keyword.IdentityFile, "abc2");


            host.WhereParam(Keyword.IdentityFile)
                .ToList()
                .All(param => param.Argument.StartsWith("abc"))
                .ShouldBeTrue();
            host.IdentityFile.ShouldEqual("abc1");
            host.Remove(Keyword.IdentityFile);
            host.Has(Keyword.IdentityFile).ShouldBeTrue();
            host.Remove(Keyword.IdentityFile);
            host.Has(Keyword.IdentityFile).ShouldBeFalse();
        }

        [Fact]
        public void TestAddNodeToNode()
        {
            var host = new HostNode("test");
            host.Set(Keyword.User, "user").IsFailed.ShouldBeFalse();
            host.Set(Keyword.Host, host).IsFailed.ShouldBeTrue();
        }

        /// Compiled vs Not compiled actions
        [Fact]
        public void TestCompiledNode()
        {
            var config = LoadConfig("configs/config");
            //Get a host, that is independent of the config
            var node = config.Find("dummy");
            //Change that host -> config does not reflect changes
            node.HostName = "known";
            Assert.NotEqual("known", config["dummy"].HostName);
            //Set the host to the config -> Now the changes should be reflected in the config
            config["dummy"].Count.ShouldEqual(1);
            config.Set(Keyword.Host, node);
            config["dummy"].Count.ShouldEqual(node.Count);
            //Change the config -> The compiled host should be unchanged
            config["dummy"].Port = 321;
            config["dummy"].Port.ShouldEqual((ushort) 321);
            node.Port.ShouldEqual((ushort)321);
        }

        /// Using LINQ it should be able to extract multiple Parameters
        [Fact]
        public void TestReadMultiParameters()
        {
            var config = LoadConfig("configs/config");
            var host = config.Find("eva");

            host.IdentityFile.ShouldEqual("~/.ssh/id_rsa-cert.pub");
            var identityFiles = host
                .WhereParam(Keyword.IdentityFile)
                .SelectArg()
                .ToList();
            identityFiles[0].ShouldEqual("~/.ssh/id_rsa-cert.pub");
            identityFiles[1].ShouldEqual("~/.ssh/id_rsa");
        }

        /// One should be able to get the exact directive by indexing / name / LINQ and all Matching ones per command
        [Fact]
        public void TestConfigGetters()
        {
            var config = LoadConfig("configs/config");

            //Config should return a list of parameters
            // All nodes (parameters and directives)
            config.Count.ShouldEqual(8);

            //Hosts
            config.Count(p => p.IsHost()).ShouldEqual(4);
            config.Hosts().Count.ShouldEqual(4);
            config.Nodes().Count.ShouldEqual(6);
            
            // Matching names
            var matching = config.GetAll("dummy");
            matching.Count.ShouldEqual(3);

            // Exact names
            config.Has("eva").ShouldEqual(true);
            config.Has("ave").ShouldEqual(false);
            config.IndexOf("eva").ShouldEqual(3);
            config["eva"].MatchString.ShouldEqual("eva");
            var eva = config.Get("eva");
            eva.User.ShouldEqual("evauser");
            Assert.Null(eva.HostName);
        }

        [Fact]
        public void TestConfigAdders()
        {
            var config = new SshConfig();

            // Host (insert at position or add at the very beginning using indexing)
            Assert.Null(config["testhost"]);
            var res = config.Insert(0, Keyword.Host, "testhost");
            res.IsSuccess.ShouldBeTrue();
            config.Has("testhost");

            // Match
            config.Matches().Count.ShouldEqual(0);
            var res2 = config.Set(Keyword.Match, "");
            res2.IsSuccess.ShouldBeTrue();
            config.Matches().Count.ShouldEqual(1);
        }

        [Fact]
        public void TestConfigRemovers()
        {
            var config = LoadConfig("configs/config");
            config[0].Is(Keyword.IdentitiesOnly).ShouldBeTrue();
            config.Remove(Keyword.IdentitiesOnly);
            config[0].Is(Keyword.IdentitiesOnly).ShouldBeFalse();
            
            config.Count(p => p.IsMatch()).ShouldEqual(2);
            config.RemoveAt(3);
            config.Count(p => p.IsMatch()).ShouldEqual(1);
            config.Remove(config.First(p => p.IsMatch()));
            config.Count(p => p.IsMatch()).ShouldEqual(0);

            config.PushHost("test1");
            config.PushHost("test2");
            
            config.Count(p => p.IsHost()).ShouldEqual(6);
            config.Remove(p => p.IsHost(), 1);
            config.Count(p => p.IsHost()).ShouldEqual(5);
            config.Remove("eva", options:MatchingOptions.MATCHING);
            config.Count(p => p.IsHost()).ShouldEqual(3);
            config.Remove<HostNode>(h => true, 1);
            config.Count(p => p.IsHost()).ShouldEqual(2);
            config.Remove<HostNode>(h => true);
            config.Count(p => p.IsHost()).ShouldEqual(0);
        }

        [Fact]
        public void TestPushConfigBuild()
        {
            var config = new SshConfig()
                .PushHost("host1", host =>
                {
                    host.User = "user1";
                    host.Comments.Add("This is a test host");
                })
                .PushMatch(match =>
                {
                    match.Set(Criteria.All);
                    match.User = "user3";
                })
                .PushHost("host2", host => { host.Set(Keyword.User, "user2"); });
            
            var host1 = config.Find("host1");
            host1.User.ShouldEqual("user3");
            
            var host2 = config.Find("host2");
            host2.User.ShouldEqual("user2");
        }

        /// One should be able to enumerate the config and look through all nodes (directives and direct parameters) using LINQ
        [Fact]
        public void TestEnumerableConfig()
        {
            var config = LoadConfig("configs/config");

            config.Count(param => param.IsHost())
                .ShouldEqual(4);
            config.Count(param => param.IsMatch())
                .ShouldEqual(2);
        }

        /// One should be able to create a new config
        [Fact]
        public void TestCreateAFreshConfig()
        {
            var config = new SshConfig();
            Assert.NotNull(config);
        }

        [Fact]
        public void TestFindAll()
        {
            var config = LoadConfig("configs/config");
            config.GetAll("dummy")
                .ElementAt(2)
                .User
                .ShouldEqual("defaultuser");

            var list = config.GetAll();
            list.Count.ShouldEqual(7);
            var compiledConfig = config.Compile();
            compiledConfig.GetAll().Count.ShouldEqual(10);
        }

        [Fact]
        public void TestComments()
        {
            var config = LoadConfig("configs/config");
            var dummyParam = config
                .First(p => p.Argument is HostNode {MatchString: "dummy"});
            var dummyCompiledHost = config.Find("dummy");
            var dummyConfigHost = config.GetAll("dummy")[1] as HostNode;
            Assert.NotNull(dummyCompiledHost);
            Assert.NotNull(dummyConfigHost);

            // Test connection to parameter + backing field
            dummyParam.Comments.Count.ShouldEqual(1);
            dummyCompiledHost.Comments.Count.ShouldEqual(2);
            dummyCompiledHost.IsConnected.ShouldBeFalse();
            dummyConfigHost.Comments.Count.ShouldEqual(1);
            dummyConfigHost.IsConnected.ShouldBeTrue();
            
            dummyParam.Comments[0].ShouldEqual("## dummy host");
            dummyConfigHost.Comments[0].ShouldEqual("## dummy host");

            //Add comment
            dummyParam.Comments.Insert(0, "## TestComment");
            dummyCompiledHost.Comments.Count.ShouldEqual(2);
            dummyConfigHost.Comments[0].ShouldEqual("## TestComment");
            dummyConfigHost.Comments.Count.ShouldEqual(2);
            dummyCompiledHost.Comments.Insert(0, "Compiled test");
            
            foreach (var comment in dummyConfigHost.Comments)
                comment.StartsWith("##").ShouldBeTrue();

            config.Set(Keyword.Host, dummyCompiledHost);
            dummyCompiledHost.IsConnected.ShouldBeTrue();
            config.Get("dummy").Comments[0].ShouldEqual("Compiled test");
        }

        [Fact]
        public void TestDataTypes()
        {
            var config = LoadConfig("configs/config");
            config.Get(Keyword.IdentitiesOnly).ShouldEqual(true);
            config.IdentitiesOnly.ShouldEqual(true);
        }
    }
}