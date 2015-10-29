using System.IO;
using Shadowsocks.Model;
using Shadowsocks.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Shadowsocks.Controller
{
    public class ShadowsocksController
    {
        private Thread _ramThread;

        private List<Listener> _listeners = new List<Listener>();

        private Configuration _config;
        private bool stopped = false;
        
        public class PathEventArgs : EventArgs
        {
            public string Path;
        }
        
        public event ErrorEventHandler Errored;

        public ShadowsocksController()
        {
            _config = Configuration.Load();
            StartReleasingMemory();
        }

        public bool IsRunning { get { return _listeners.Count > 0; } }

        public void Start()
        {
            Reload();
        }

        protected void Reload()
        {
            // some logic in configuration updated the config when saving, we need to read it again
            Configuration newConfig = Configuration.Load();
            
            HashSet<int> newPortsAdded = new HashSet<int>();
            bool anyPortInUse = false;
            foreach (ServerInfo si in newConfig.configs)
            {
                if (Utils.CheckIfPortInUse(si.local_port))
                {
                    Console.WriteLine("Service " + si.FriendlyName() + " uses a local port " + si.local_port.ToString() + " currently already in use.");
                    anyPortInUse = true;
                }
                else if (newPortsAdded.Contains(si.local_port))
                {
                    Console.WriteLine("Service " + si.FriendlyName() + " uses a local port " + si.local_port.ToString() + " used by another service.");
                    anyPortInUse = true;
                }
                else
                {
                    newPortsAdded.Add(si.local_port);
                }
            }

            if (anyPortInUse)
            {
                Console.WriteLine("Unless all services can have their local ports available, no action is done.");
            }
            else
            {
                _config = newConfig;

                foreach (var l in _listeners)
                    l.Stop();

                try
                {
                    foreach (ServerInfo si in _config.configs)
                    {
                        TCPRelay tcpRelay = new TCPRelay(si);
                        UDPRelay udpRelay = new UDPRelay(si);
                        List<Listener.Service> services = new List<Listener.Service>();
                        services.Add(tcpRelay);
                        services.Add(udpRelay);

                        Listener li = new Listener(services);
                        _listeners.Add(li);

                        li.Start(si);
                    }
                }
                catch (Exception e)
                {
                    // translate Microsoft language into human language
                    // i.e. An attempt was made to access a socket in a way forbidden by its access permissions => Port already in use
                    if (e is SocketException)
                    {
                        SocketException se = (SocketException)e;
                        if (se.SocketErrorCode == SocketError.AccessDenied)
                        {
                            e = new Exception("Port already in use", e);
                        }
                    }
                    Logging.LogUsefulException(e);
                    ReportError(e);
                }
            }

            Util.Utils.ReleaseMemory(true);
        }

        protected void ReportError(Exception e)
        {
            if (Errored != null)
            {
                Errored(this, new ErrorEventArgs(e));
            }
        }
        
        // always return copy
        public Configuration GetConfigurationCopy()
        {
            return Configuration.Load();
        }

        // always return current instance
        public Configuration GetCurrentConfiguration()
        {
            return _config;
        }
        
        public void SaveServers(List<ServerInfo> servers, int localPort)
        {
            _config.configs = servers;
            SaveConfig(_config);
        }

        public bool AddServerBySSURL(string ssURL)
        {
            try
            {
                var server = new ServerInfo(ssURL);
                _config.configs.Add(server);
                SaveConfig(_config);
                return true;
            }
            catch (Exception e)
            {
                Logging.LogUsefulException(e);
                return false;
            }
        }
        
        public void Stop()
        {
            if (stopped)
            {
                return;
            }
            stopped = true;

            foreach (var l in _listeners)
                l.Stop();
        }
        
       

        protected void SaveConfig(Configuration newConfig)
        {
            Configuration.Save(newConfig);
            Reload();
        }
        
        private void StartReleasingMemory()
        {
            _ramThread = new Thread(new ThreadStart(ReleaseMemory));
            _ramThread.IsBackground = true;
            _ramThread.Start();
        }

        private void ReleaseMemory()
        {
            while (true)
            {
                Util.Utils.ReleaseMemory(false);
                Thread.Sleep(30 * 1000);
            }
        }
    }
}
