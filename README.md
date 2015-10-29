Shadowsocks for Windows - Lite
=======================

Bare minimum Shadowsocks commandline client with multiple servers, each has a listening port for sock5 connection, suitable for a server in LAN. All other features from shadowsocks-windows were removed.

To install as a Windows service, use a wrapper like [winsw].

To access one port as a http proxy, use privoxy similar to the way shadowsocks-windows did. Sample config included in "service".

#### Server Configuration

```
{
"configs" : 
[
	{
		"server" : "ip1",
		"server_port" : port,
		"shareOverLan" : true,
		"local_port" : client port 1,
		"password" : "password",
		"method" : "encrpytion method",
		"remarks" : "server 1"
	},

	{
		"server" : "ip2",
		"server_port" : port,
		"shareOverLan" : true,
		"local_port" : client port 2,
		"password" : "password",
		"method" : "aes-256-cfb",
		"remarks" : "server 2"
	},

	...

	{
		"server" : "ip n",
		"server_port" : port,
		"shareOverLan" : true,
		"local_port" : client port n,
		"password" : "password",
		"method" : "aes-256-cfb",
		"remarks" : "server n"
	}
],
}
```

#### Develop

Visual Studio 2015 is required.

#### License

GPLv3

[winsw]:	https://github.com/kohsuke/winsw
