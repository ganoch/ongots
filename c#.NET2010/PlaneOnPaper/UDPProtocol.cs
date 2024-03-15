using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PlaneOnPaper
{
    public class UDPProtocol 
    {
        public static bool IsListening = false;

        private static Thread udpListenThread;
        private static UdpClient udp_client;

        public static void StartUDPListener()
        {
            udpListenThread = new Thread(new ThreadStart(UDPProtocol.ListenForUDP));
            udpListenThread.Start();
        }

        public static void StopUDPListener()
        {
            if (udp_client != null  && udpListenThread != null && udpListenThread.IsAlive && udpListenThread.ThreadState == ThreadState.Running)
            {
                udp_client.Close();
                udpListenThread.Interrupt();
                IsListening = false;
            }
        }

        public static void translateMessage(byte[] message, IPEndPoint client_ep )
        {
            switch ((int)message[0])
            {
                case 1: //request server info
                    if(PlaneServer.TheServer != null)
                        PlaneServer.TheServer.SendServerInfo(client_ep);
                    break;

                case 2: //server info
                    if(PlaneClient.TheClient != null)
                        PlaneClient.TheClient.ReceiveServerInfo((int)message[1], Encoding.UTF8.GetString(message,2,message.Length -2), client_ep);
                    break;
            }
        }

        public static void SendUDPMessage(byte[] message, IPEndPoint client_ep)
        {
            IPEndPoint localpt = new IPEndPoint(IPAddress.Any, Configuration.ListenningPort);
            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(localpt);
            client.Send(message, message.Length, client_ep);
            System.Diagnostics.Debug.WriteLine("UDP.snt: " + client_ep.ToString()+ " value:"+Encoding.ASCII.GetString(message));
        }

        public static void ListenForUDP()
        {
            try
            {
                IPEndPoint localpt = new IPEndPoint(IPAddress.Any, Configuration.ListenningPort);
                udp_client = new UdpClient();
                udp_client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udp_client.Client.Bind(localpt);

                IPEndPoint client_ep = new IPEndPoint(IPAddress.Any, 0);

                System.Diagnostics.Debug.WriteLine("UDP.lstn: " + Configuration.ListenningPort);
                IsListening = true;
                while (true)
                {
                    try
                    {
                        byte[] received_bytes = udp_client.Receive(ref client_ep);
                        System.Diagnostics.Debug.WriteLine("UDP.rcv: " +client_ep.ToString() + " value: "+ Encoding.ASCII.GetString(received_bytes));
                        UDPProtocol.translateMessage(received_bytes, client_ep);
                    }
                    catch (ThreadInterruptedException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    System.Diagnostics.Debug.WriteLine("Port not open");
                }
            }

        }
    }
}
