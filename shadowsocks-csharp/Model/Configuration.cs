using Shadowsocks.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using System.Windows.Forms;

namespace Shadowsocks.Model
{
    [Serializable]
    public class Configuration
    {
        public List<ServerInfo> configs = new List<ServerInfo>();
        
        private static string CONFIG_FILE = "configs.json";
        
        public static void CheckServer(ServerInfo server)
        {
            CheckPort(server.server_port);
            CheckPassword(server.password);
            CheckServer(server.server);
        }

        public static Configuration Load()
        {
            try
            {
                string configContent = File.ReadAllText(CONFIG_FILE);
                Configuration config = SimpleJson.SimpleJson.DeserializeObject<Configuration>(configContent, new JsonSerializerStrategy());
               
                return config;
            }
            catch (Exception )
            {
            }
            return new Configuration();
        }

        public static void Save(Configuration config)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(CONFIG_FILE, FileMode.Create)))
                {
                    string jsonString = SimpleJson.SimpleJson.SerializeObject(config);
                    sw.Write(jsonString);
                    sw.Flush();
                }
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public static ServerInfo GetDefaultServer()
        {
            return new ServerInfo();
        }

        private static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("assertion failure");
            }
        }

        public static void CheckPort(int port)
        {
            if (port <= 0 || port > 65535)
            {
                throw new ArgumentException("Port out of range");
            }
        }

        public static void CheckLocalPort(int port)
        {
            CheckPort(port);
            if (port == 8123)
            {
                throw new ArgumentException("Port can't be 8123");
            }
        }

        private static void CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password can not be blank");
            }
        }

        private static void CheckServer(string server)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentException("Server IP can not be blank");
            }
        }

        private class JsonSerializerStrategy : SimpleJson.PocoJsonSerializerStrategy
        {
            // convert string to int
            public override object DeserializeObject(object value, Type type)
            {
                if (type == typeof(Int32) && value.GetType() == typeof(string))
                {
                    return Int32.Parse(value.ToString());
                }
                return base.DeserializeObject(value, type);
            }
        }
    }
}
