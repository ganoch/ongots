using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace PlaneOnPaper
{
    public class Configuration
    {
        private static int _listen_port = 44502;
        private static int _used_listen_port = 0;
        public static int ListenPort { get { return _listen_port; } }
        public static int ListenningPort { 
            get 
            {
                if (_used_listen_port == 0)
                {
                    bool isAvailable = true;
                    IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                    IPEndPoint[] udp_listeners = ipGlobalProperties.GetActiveUdpListeners();

                    foreach (IPEndPoint ep in udp_listeners)
                    {
                        if (ep.Port == Configuration._listen_port)
                        {
                            isAvailable = false;
                            break;
                        }
                    }
                    _used_listen_port = isAvailable ? Configuration._listen_port : Configuration._listen_port + 1; 
                }
                return _used_listen_port;
            } 
        }
        public static int OutPort = 44501;
    }
}
