using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Drawing;
using System.Threading;


namespace PlaneOnPaper
{
    public class TCPOpponent : APossibilityOpponent
    {
        TcpClient tcp_client = null;
        private int _my_planes_count = 0;
        private int _total_planes = 0;
        private string _name = "";

        public TCPOpponent(TcpClient client, string name)
        {
            this._name = name;
            this.tcp_client = client;
        }

        public override bool Defeated
        {
            get { return this._shot_down_count == this._my_planes_count; }
        }

        public override bool PlacePlanes(int number_of_planes)
        {
            this._total_planes = this._my_planes_count = number_of_planes;
            return this.PlanesArePlaced;
        }

        private bool awaiting_shot = false;
        public override byte ShotAt(System.Drawing.Point coordinates)
        {
            this.awaiting_shot = true;

            if ((this._display_board[coordinates.X + coordinates.Y * 10] & 4) == 4)
                this.CurrentShot = 7;
            else
                TCPProtocol.SendShotCoor(coordinates);

            while (this.awaiting_shot)
            {
            }

            if (this._current_shot != 7)
            {
                this._display_board[coordinates.X + coordinates.Y * 10] = this._current_shot;

                if (this._display_board[coordinates.X + coordinates.Y * 10] == 6)
                {
                    this._shot_down_count++;
                }
            }

            return this._current_shot;
        }

        private byte _current_shot = 0;
        public byte CurrentShot
        { 
            set
            {
                this._current_shot = value;
                this.awaiting_shot = false;
            }
        }

        //mandatory
        public override string Name
        {
            get { return this._name; }
        }


        public volatile byte ShotCoordinates = 101;
        public override bool MakeShot()
        {
           
            if (ShotCoordinates > 100)
                return false;

            byte shot_returned = this.ShootEnemyHandle(new Point(ShotCoordinates % 10, ShotCoordinates / 10));
            TCPProtocol.SendShotResult((byte)ShotCoordinates, shot_returned);
            this.ShotCoordinates = 101;
            
            if (shot_returned == 7)
                return false;
            return true;
                
        }

        public byte[] Hash
        { get; set; }

        public byte[] InitialBoard { get; set; }
        public Plane[] Planes { get; set; }

        
    }
}
