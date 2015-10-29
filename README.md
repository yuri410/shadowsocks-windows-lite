Shadowsocks for Windows - Lite
=======================

Commandline client with multiple server connections

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


