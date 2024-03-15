using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading;

namespace PlaneOnPaper
{
    public class PlaneServerInfo
    {
        public string Name;
        public IPEndPoint ServerEndPoint;
        public int PlaneCount;
        public override string ToString()
        {
            return this.Name + " (" + this.ServerEndPoint.Address.ToString() + ")" + "          " + this.PlaneCount +" онгоц";
        }
    }

    class TCPConnectionEstablishedEventArgs : EventArgs
    {
        TCPConnectionEstablishedEventArgs(TcpClient client)
        {
            this.TcpClient = client;
        }
        public TcpClient TcpClient { get; set; }
    }

    class PlaneClient
    {
        private Dictionary<IPEndPoint,PlaneServerInfo> infos = new Dictionary<IPEndPoint,PlaneServerInfo>();

        public static PlaneClient TheClient
        {
            get;
            set;
        }

        public PlaneClient()
        {
            PlaneClient.TheClient = this;
            
            List<IPAddress> broadcasts = new List<IPAddress>();
            NetworkInterface[] adapters  = NetworkInterface.GetAllNetworkInterfaces();


            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    IPv4InterfaceProperties interfaceProperties = adapterProperties.GetIPv4Properties();

                    IPv4InterfaceStatistics ip4statistics = adapter.GetIPv4Statistics();
                    foreach(UnicastIPAddressInformation unicast in adapterProperties.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily == AddressFamily.InterNetwork && unicast.IPv4Mask != null)
                        {

                            byte[] address = unicast.Address.GetAddressBytes();
                            byte[] mask = unicast.IPv4Mask.GetAddressBytes();
                            byte[] broadcast_ = address;

                            int i=0;
                            foreach (byte b in address)
                            {
                                broadcast_[i] = (byte)(b | ~mask[i]);
                                i++;
                            }
                            broadcasts.Add(new IPAddress(broadcast_));
                            break;
                        }
                    }
                    
                }
            }

            UDPProtocol.StartUDPListener();

            while (!UDPProtocol.IsListening)
            { }
            SendBroadcastMessage(broadcasts.ToArray());

            connectDialog dlg = new connectDialog(ref this.infos);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg.listView1.SelectedItems.Count > 0 && dlg.listView1.SelectedItems[0] != null)
            {
                UDPProtocol.StopUDPListener();
               
                    TCPProtocol.SendClientInfo(Game.Me.Name, (PlaneServerInfo)dlg.listView1.SelectedItems[0].Tag);
               
           
                  
            }
            else
            {
                UDPProtocol.StopUDPListener();
            }

            
            infos.Clear();
            
        }

        public delegate void TCPConnectionEstablishedEventHandler(object sender, TCPConnectionEstablishedEventArgs e);

        public event TCPConnectionEstablishedEventHandler TCPConnectionEstablished;

        public virtual void OnTCPConnectionEstablished(TCPConnectionEstablishedEventArgs e)
        {
            if (TCPConnectionEstablished != null)
                TCPConnectionEstablished(this, e);
        }

        public void ReceiveServerInfo(int plane_count, string server_name, IPEndPoint server_ep)
        {
            infos.Add(server_ep, new PlaneServerInfo(){ Name = server_name, PlaneCount = plane_count, ServerEndPoint = server_ep });

            System.Diagnostics.Debug.WriteLine("Server info received: "+server_name + " " + plane_count + " planes from " + server_ep.ToString()+":"+server_ep.Port);
        }

        public void SendBroadcastMessage(IPAddress[] broadcasts)
        {
            //IPEndPoint localpt = new IPEndPoint(IPAddress.Any, Configuration.ListenningPort);
            //UdpClient client = new UdpClient();
            //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //client.Client.Bind(localpt);

            foreach (IPAddress broadcast in broadcasts)
            {
                IPEndPoint serverEndPoint = new IPEndPoint(broadcast, Configuration.ListenPort);

                UDPProtocol.SendUDPMessage(new byte[] { 1 }, serverEndPoint);

                System.Diagnostics.Debug.WriteLine("Sending broadcast to port: " + Configuration.ListenPort + " using port:" + Configuration.ListenningPort);
            }
        }

        
    }
}
