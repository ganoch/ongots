using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.IO;

namespace PlaneOnPaper
{

    public class TCPProtocol
    {
        private static TcpClient client;
        private static TcpListener tcpListener;
        private static Thread listenThread;
        private static Thread commThread;
        private static NetworkStream clientStream;

        public static bool IsListening = false;

        private static bool _is_server;
        public static bool IsServer 
        { 
            get 
            { return TCPProtocol._is_server; } 
            private set 
            { 
                TCPProtocol._is_server = value; 
                Game.IsServer = value;
                Game.IsNetwork = true;
            } 
        }

        public static void StartTCPListener()
        {
            TCPProtocol.tcpListener = new TcpListener(IPAddress.Any, Configuration.ListenningPort);
            TCPProtocol.listenThread = new Thread(new ThreadStart(ListenForTCP));
            TCPProtocol.listenThread.Start();
            
        }

        public static void StopTCPListener()
        {
            if (TCPProtocol.commThread != null && TCPProtocol.commThread.IsAlive && TCPProtocol.commThread.ThreadState == ThreadState.Running)
                TCPProtocol.commThread.Interrupt();

            System.Diagnostics.Debug.WriteLine("Closing TCP");
            try
            {
                if(TCPProtocol.clientStream != null)
                    TCPProtocol.clientStream.Close();
                if (TCPProtocol.client != null)
                {
                    if (TCPProtocol.client.Client != null)
                    {
                        if (TCPProtocol.client.Client.Connected)
                        {
                            TCPProtocol.tcpListener.Server.Shutdown(SocketShutdown.Both);
                            TCPProtocol.client.Client.Disconnect(false);
                        }
                        TCPProtocol.client.Client.Close();
                    }
                    TCPProtocol.client.Close();
                }

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debug.WriteLine("attempt close client, disposed exception");
            }


            TCPProtocol.Available = false;
            if (TCPProtocol.listenThread != null && TCPProtocol.listenThread.IsAlive && TCPProtocol.listenThread.ThreadState == ThreadState.Running)
                TCPProtocol.listenThread.Interrupt();

            if (TCPProtocol.tcpListener != null)
            {
                if (TCPProtocol.tcpListener.Server.Connected)
                {
                    TCPProtocol.tcpListener.Server.Shutdown(SocketShutdown.Both);
                    TCPProtocol.tcpListener.Server.Disconnect(false);
                }
                TCPProtocol.tcpListener.Server.Close();
                TCPProtocol.tcpListener.Stop();
                
               
            }




        }

        public static volatile bool Available = true;
        public static void ListenForTCP()
        {
            TCPProtocol.IsServer = true;
            TCPProtocol.Available = true;
            System.Diagnostics.Debug.WriteLine("TCP.lstn:"+TCPProtocol.tcpListener.LocalEndpoint.ToString());
            try
            {
                TCPProtocol.tcpListener.Start();
                //TCPProtocol.tcpListener.Server.Blocking = false;
                while (TCPProtocol.Available)
                {
                    //blocks until a client has connected to the server
                    TCPProtocol.client = TCPProtocol.tcpListener.AcceptTcpClient();


                    System.Diagnostics.Debug.WriteLine("TCP.clnt: " + TCPProtocol.client.Client.RemoteEndPoint);
                    //create a thread to handle communication 
                    //with connected client
                    TCPProtocol.commThread = new Thread(new ParameterizedThreadStart(HandleComm));
                    TCPProtocol.commThread.Start(TCPProtocol.client);
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    System.Diagnostics.Debug.WriteLine("Port not open");
                }
                else if (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    System.Diagnostics.Debug.WriteLine("Closing listener");
                }
                else
                    throw ex;
            }

        }

        public static void ConnectTCP(IPEndPoint server_ep)
        {
            TCPProtocol.IsServer = false;

            System.Diagnostics.Debug.WriteLine("TCP.cnct: " + server_ep.ToString());

            
                TCPProtocol.client = new TcpClient();
                TCPProtocol.client.Connect(server_ep);



                TCPProtocol.commThread = new Thread(new ParameterizedThreadStart(HandleComm));
                TCPProtocol.commThread.Start(TCPProtocol.client);
            


        }

        private static void HandleComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            TCPProtocol.clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (tcpClient.Connected)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);

                }
                catch (ObjectDisposedException)
                {
                    Game.GameStatus = GameStatuses.Disconnected;
                    break;
                }
                catch (IOException ex)
                {
                    if (!(ex.InnerException is SocketException))
                        throw ex;

                    SocketException inner_ex = (SocketException)ex.InnerException;
                    if (inner_ex.SocketErrorCode == SocketError.Interrupted
                     || inner_ex.SocketErrorCode == SocketError.ConnectionAborted
                     || inner_ex.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        Game.GameStatus = GameStatuses.Disconnected;
                        break;
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    Game.GameStatus = GameStatuses.Disconnected;
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }


                byte[] actual_message = new byte[bytesRead];
                Array.Copy(message, 0, actual_message, 0, bytesRead);
                TCPProtocol.translateMessage(actual_message, tcpClient);
            }

            tcpClient.Close();
        }

        public static void translateMessage(byte[] message, TcpClient client)
        {

#if DEBUG
            System.Diagnostics.Debug.Write("TCP.rcv: " + client.Client.RemoteEndPoint.ToString() + " " + message[0].ToString() + ":");
            DisplayMessage(message);
#endif


            switch ((int)message[0])
            {
                case 1:
                    if (TCPProtocol.IsServer)
                    {
                        byte[] msg = new byte[4];
                        msg[0] = (byte)2;
                        Array.Copy(Game.MyHalf, 0, msg, 1, 3);
                        TCPProtocol.SendMessage(msg);

                        Game.Opponent = new PlaneOnPaper.TCPOpponent(client, Encoding.UTF8.GetString(message,1,message.Length -1));
                    }
                    
                    break;
                
                case 2:
                    int salt_length = message.Length - 1;
                    byte[] other_half = new byte[salt_length];
                    Array.Copy(message, 1, other_half, 0, salt_length);
                    Game.OtherHalf = other_half;

                    if (!TCPProtocol.IsServer)
                    {
                        byte[] msg = new byte[1 + Game.MyHalf.Length];
                        msg[0] = (byte)2;
                        Array.Copy(Game.MyHalf, 0, msg, 1, Game.MyHalf.Length);
                        TCPProtocol.SendMessage(msg);
                        Game.Me.PlanesPlaced += new EventHandler(Me_PlanesPlaced);
                    }
                    else
                    {
                        TCPProtocol.SendMyHash();
                    }
                    Game.GameStatus = GameStatuses.PlacePlanes;
                    break;

                case 3:
                    Game.Opponent.PlanesArePlaced = true;
                    ((TCPOpponent)Game.Opponent).Hash = new byte[message.Length - 1];
                    Array.Copy(message, 1, ((TCPOpponent)Game.Opponent).Hash, 0, message.Length - 1);
                    break;

                case 4:
                    if (!Game.IsServer)
                        Game.MyTurn = message[1] == 0 ? 1 : 0;
                    break;

                case 5:
                    ((TCPOpponent)Game.Opponent).ShotCoordinates = message[1];
                    break;


                case 6:
                    ((TCPOpponent)Game.Opponent).CurrentShot = message[2];
                    break;

                case 7:
                    ((TCPOpponent)Game.Opponent).InitialBoard = new byte[100];
                    Array.Copy(message, 1, ((TCPOpponent)Game.Opponent).InitialBoard, 0, 100);

                    int plane_cnt =  (message.Length - 101)/2;
                    ((TCPOpponent)Game.Opponent).Planes = new Plane[plane_cnt];
                    for (int i = 101; i < message.Length; i += 2)
                    {
                        Plane pl = new Plane(Color.Red, (message[i] % 10), (message[i] / 10), message[i+1]);
                        ((TCPOpponent)Game.Opponent).Planes[--plane_cnt] = pl;
                    }

                    Game.CheckValidity();
                    break;

            }


        }

#if DEBUG
        private static void DisplayMessage(byte[] message)
        {
            switch ((int)message[0])
            {
                case 1:
                    System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(message, 1, message.Length - 1));
                    break;

                case 2:
                    int salt_length = message.Length - 1;
                    byte[] other_half = new byte[salt_length];

                    Array.Copy(message, 1, other_half, 0, salt_length);
                    System.Diagnostics.Debug.Write(GetHexString(other_half));
                    System.Diagnostics.Debug.WriteLine(" combined salt: " + GetHexString(Game.Salt));
                    break;

                case 3:
                    byte[] hash = new byte[message.Length - 1];
                    Array.Copy(message, 1, hash, 0, message.Length - 1);
                    System.Diagnostics.Debug.WriteLine(GetHexString(hash));
                    break;

                case 4:
                    System.Diagnostics.Debug.WriteLine(message[1] == 0 ? "server's turn" : "client's turn");
                    break;

                case 5:
                    System.Diagnostics.Debug.WriteLine("(" + (message[1] % 10).ToString() + "," + (message[1] / 10).ToString() + ")");
                    break;

                case 6:
                    System.Diagnostics.Debug.WriteLine("(" + (message[1] % 10).ToString() + "," + (message[1] / 10).ToString() + ") " + (message[2] == 4 ? "х" : message[2] == 5 ? "ш" : (message[2] == 6?"с":"а")));
                    break;
                
                case 7:
                    byte[] board = new byte[100];
                    Array.Copy(message, 1, board, 0, 100);

                    string prnt = "";

                    prnt = "initial board: \r\n";
                    byte[] line = new byte[10];
                    for (int i = 0; i < 10; i++)
                    {
                        Array.Copy(board, i * 10, line, 0, 10);
                        prnt += "          " + GetHexString(line) + "\r\n";
                    }
                    
                    prnt += "     planes: ";

                    for (int i = 101; i < message.Length; i += 2)
                    {
                        if (i != 101)
                            prnt += (", ");
                        prnt += "(" + (message[i] % 10).ToString() + "," + (message[i] / 10).ToString() + ") ";
                        prnt += (message[i+1].ToString());
                    }
                    System.Diagnostics.Debug.WriteLine(prnt);
                    break;
            }
        }
#endif

        public static void SendInitialBoard(Field fld)
        {

            byte[] message =  new byte[101 + fld.PlaneCount * 2];
            message[0] = 7;
            fld.InitialBoard[RandomIndex] |= 4;
            Array.Copy(fld.InitialBoard, 0, message, 1, 100); //initial board оруулах
            int index = 101;
            foreach(Plane pl in fld.Planes)
            {
                message[index++] = (byte)(pl.Coordinates.X + pl.Coordinates.Y * 10);
                message[index++] = (byte)(pl.Direction);
            }
            TCPProtocol.SendMessage(message);
        }

        static void Me_PlanesPlaced(object sender, EventArgs e)
        {
            SendMyHash();
        }

        private static int RandomIndex = Game.Rand.Next(100);

        public static void SendMyHash()
        {

            Game.Me.InitialBoard[RandomIndex] |= 4;
            byte[] hash = Game.GetHash(Game.Me.InitialBoard);
            byte[] message = new byte[1 + hash.Length];
            message[0] = 3;
            Array.Copy(hash, 0, message, 1, hash.Length);

            TCPProtocol.SendMessage(message);
        }

        public static void SendMessage(byte[] message)
        {
            try
            {
                if (TCPProtocol.client != null)
                {
                    NetworkStream clientStream = TCPProtocol.client.GetStream();

#if DEBUG
                    System.Diagnostics.Debug.Write("TCP.snd: " + TCPProtocol.client.Client.RemoteEndPoint.ToString() + " " + message[0].ToString() + ":");
                    DisplayMessage(message);
#endif



                    clientStream.Write(message, 0, message.Length);
                    clientStream.Flush();
                }
            }
            catch (ObjectDisposedException)
            {
                Game.GameStatus = GameStatuses.Disconnected;
            }
            
        }


        public static void SendClientInfo(string name, PlaneServerInfo server_info)
        {
            try
            {
                TCPProtocol.ConnectTCP(server_info.ServerEndPoint);

                Game.GameStatus = GameStatuses.WaitingForSettings;
                Game.NumberOfPlanes = server_info.PlaneCount;


                int byte_count = Encoding.UTF8.GetByteCount(name);
                byte[] message = new byte[1 + byte_count];
                message[0] = 1;
                Array.Copy(Encoding.UTF8.GetBytes(name), 0, message, 1, byte_count);
                TCPProtocol.SendMessage(message);

                Game.Opponent = new TCPOpponent(TCPProtocol.client, server_info.Name);
            }
            catch (SocketException ex)
            {
                Game.GameStatus = GameStatuses.Disconnected;
                Game.Message = "Холбогдох боломжгүй: " + ex.Message;
            }
            catch (ObjectDisposedException)
            {
                Game.GameStatus = GameStatuses.Disconnected;

                Game.Message = "Холбогдох боломжгүй";
            }
            catch (IOException ex)
            {
                if (!(ex.InnerException is SocketException))
                    throw ex;

                SocketException inner_ex = (SocketException)ex.InnerException;
                if (inner_ex.SocketErrorCode == SocketError.Interrupted
                 || inner_ex.SocketErrorCode == SocketError.ConnectionAborted
                 || inner_ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Game.GameStatus = GameStatuses.Disconnected;
                    Game.Message = "Холбогдох боломжгүй";
                }
                else
                {
                    throw ex;
                }
            }
            catch (ThreadInterruptedException)
            {
                Game.GameStatus = GameStatuses.Disconnected;
                Game.Message = "Холбогдох боломжгүй";
            }
        }


        public static void SendTurnInfo(int myturn) //when 0 server's turn 
        {
            byte[] msg = new byte[] { 4, (byte)myturn };
            TCPProtocol.SendMessage(msg);
        }

        public static void SendShotCoor(Point coor)
        {
            byte[] msg = new byte[] { 5, (byte)(coor.X + coor.Y * 10) };
            TCPProtocol.SendMessage(msg);
        }

        public static void SendShotResult(byte coor, byte result)
        {
            byte[] msg = new byte[] { 6, coor, result };
            TCPProtocol.SendMessage(msg);
        }

        public static string GetHexString(byte[] arr)
        {
            string ret = "";

            bool is_first = true;
            foreach (byte b in arr)
            {
                if (!is_first)
                    ret += " ";
                else
                    is_first = false;
                ret += GetHex(b);
            }
            return ret;
        }

        public static string GetHex(byte b)
        {
            string ret;
            switch(b/16)
            {
                case 10:
                    ret = "A"; break;
                case 11:
                    ret = "B"; break;
                case 12:
                    ret = "C"; break;
                case 13:
                    ret = "D"; break;
                case 14:
                    ret = "E"; break;
                case 15:
                    ret = "F"; break;
                default:
                    ret = (b / 16).ToString(); break;
            }

            switch (b % 16)
            {
                case 10:
                    ret += "A"; break;
                case 11:
                    ret += "B"; break;
                case 12:
                    ret += "C"; break;
                case 13:
                    ret += "D"; break;
                case 14:
                    ret += "E"; break;
                case 15:
                    ret += "F"; break;
                default:
                    ret += (b % 16).ToString(); break;
            }
            return ret;
        }
    }
}
