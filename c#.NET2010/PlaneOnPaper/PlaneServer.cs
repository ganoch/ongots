using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace PlaneOnPaper
{
    public class PlaneServer
    {

        
        private int _plane_count;
        private string _name;

        public static PlaneServer TheServer
        {
            get; set;
        }

        public PlaneServer(int plane_count, string name)
        {
            UDPProtocol.StartUDPListener();
            TCPProtocol.StartTCPListener();


            this._plane_count = plane_count;
            this._name = name;

            PlaneServer.TheServer = this;
        }

        public int PlaneCount { get { return Game.NumberOfPlanes; } }
        public string ServerName { get { return Game.Me.Name; } }

        public void SendServerInfo(IPEndPoint client_ep)
        {
            int byte_count = Encoding.UTF8.GetByteCount(this.ServerName);
            byte[] message = new byte[2 + byte_count];
            message[0] = 2;
            message[1] = (byte)this.PlaneCount;
            Array.Copy(Encoding.UTF8.GetBytes(this.ServerName), 0, message, 2, byte_count);

            UDPProtocol.SendUDPMessage(message, client_ep);
        }

       

        
    }
}
