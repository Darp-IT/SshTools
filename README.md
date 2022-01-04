# SshTools

## Introduction

The ssh config file is a very powerful tool to specify a ssh connection.
In C# we have access to the awesome ssh library [SSH.NET](https://github.com/sshnet/SSH.NET),
but this library does not aim for providing config file parsing support or a very intuitive way of defining hosts.
Inspired by the [Ssh-Config-Parser](https://github.com/JeremySkinner/Ssh-Config-Parser) library the goal was to
provide a library, that links the world of config files to ssh in c#.

## Features

- [x] Configure custom Keywords, Tokens or Criteria
- [x] Parse or create a new SshConfig
- [x] Edit this config and it's Hosts / Matches / Arguments
- [x] Find a matching Host
- [ ] Use this Host to connect with the help of Ssh.Net
- [x] Save the edited config back to the disk
- [x] Usage of [FluentResults](https://github.com/altmann/FluentResults) to explain failure
- [x] Using IEnumerable as base for the config, so LINQ can be used to query parameters

## Usage
### Creating a new config
A new config can be created directly from a ssh file on the disk
```cs
var configRes = SshConfig.FromFile("path/to/config");
if (configRes.IsFailed)
{
    configRes.Errors; // Parsing errors can be found here
    return;
}
var config = configRes.Value;
```
Alternatively a config can be created from a string representing the config
```cs
var configRes = SshConfig.Deserialize("Host test\n  User testuser");
if (configRes.IsFailed)
{
    configRes.Errors; // Parsing errors can be found here
    return;
}
var config = configRes.Value;
var testHost = config.Find("test"); // An uncoupled host containing the "User" argument
var testUser = testHost.User; // testuser
```
A config can also be created directly from code
```cs
var config = new SshConfig();
```

### Editing the config

A new Argument can be inserted easily anywhere by providing
- index (negative numbers will count from the end)
- The argument's Keyword (e.g. User, Port, Host, etc)
- The value, that is complementary to the Keyword's expected type
```cs
var config = new SshConfig();
var insertionRes = config.Insert<ushort>(0, Keyword.Port, 1234);
```
Nodes (Host / Match) can be inserted too, but there is an overload, where the matchString can be defined
```cs
var config = new SshConfig();
config.Insert(-1, Keyword.Match, "user testuser host testhost");
```
Additionally there are many other ways, that provide a cleaner way to edit
```cs
var config = new SshConfig();
config.Port = 1234;
config.Set<ushort>(Keyword.Port, 1234);
config.PushHost("hostName", host =>
{
    host.User = "username";
});
```
Of course values can be removed again
```cs
var config = new SshConfig();
config.Port = 1234;
config.Remove(Keyword.Port);

config.PushHost("testHost");
config.Remove(parameter => parameter.IsHost());
```

### Getting values from the config

Values can be queried in various different ways
```cs
var config = new SshConfig();
config.Port = 123;
config.IdentityFile = "~/.ssh/id_rsa1"
config.Insert(-1, Keyword.IdentityFile, "~/.ssh/id_rsa2");

var port = config.Port;
port = config.Get<ushort>(Keyword.Port);

var numberOfIdentities = config.Count(parameter => parameter.Is(Keyword.IdentityFile));
// [ "~/.ssh/id_rsa1", "~/.ssh/id_rsa2" ]
var identityFiles = config.WhereParam(Keyword.IdentityFile).ToList();
var id_rsa2 = config
    .WhereParam(Keyword.IdentityFile)
    .SelectArg()
    .First(fileName => fileName.EndsWith("2"));
```
### Export the config
```cs
var config = new SshConfig();
config.PushHost("hostName");
config.WriteFile("path/to/output/config");
```