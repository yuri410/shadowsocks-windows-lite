using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using SimpleJson;
using Shadowsocks.Controller;
using System.Text.RegularExpressions;

namespace Shadowsocks.Model
{
    [Serializable]
    public class ServerInfo
    {
        public string server;
        public int server_port;
        public bool shareOverLan;
        public int local_port;
        public string password;
        public string method;
        public string remarks;

        public override int GetHashCode()
        {
            return server.GetHashCode() ^ server_port;
        }

        public override bool Equals(object obj)
        {
            ServerInfo o2 = (ServerInfo)obj;
            return this.server == o2.server && this.server_port == o2.server_port;
        }

        public string FriendlyName()
        {
            if (string.IsNullOrEmpty(server))
            {
                return "New server";
            }
            if (string.IsNullOrEmpty(remarks))
            {
                return server + ":" + server_port;
            }
            else
            {
                return remarks + " (" + server + ":" + server_port + ")";
            }
        }

        public ServerInfo()
        {
            this.server = "";
            this.server_port = 8388;
            this.method = "aes-256-cfb";
            this.password = "";
            this.remarks = "";
            this.local_port = 1080;
            this.shareOverLan = false;
        }

        public ServerInfo(string ssURL) : this()
        {
            string[] r1 = Regex.Split(ssURL, "ss://", RegexOptions.IgnoreCase);
            string base64 = r1[1].ToString();
            byte[] bytes = null;
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    bytes = System.Convert.FromBase64String(base64);
                }
                catch (FormatException)
                {
                    base64 += "=";
                }
            }
            if (bytes == null)
            {
                throw new FormatException();
            }
            try
            {
                string data = Encoding.UTF8.GetString(bytes);
                int indexLastAt = data.LastIndexOf('@');

                string afterAt = data.Substring(indexLastAt + 1);
                int indexLastColon = afterAt.LastIndexOf(':');
                this.server_port = int.Parse(afterAt.Substring(indexLastColon + 1));
                this.server = afterAt.Substring(0, indexLastColon);

                string beforeAt = data.Substring(0, indexLastAt);
                string[] parts = beforeAt.Split(new[] { ':' });
                this.method = parts[0];
                this.password = parts[1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new FormatException();
            }
        }
    }
}
