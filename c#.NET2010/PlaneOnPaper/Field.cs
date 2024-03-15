using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaneOnPaper
{
    public class Field
    {
        private byte[] _board;
        private string _name;
        private List<Plane> planes = new List<Plane>();

        public int PlaneCount { get { return this.planes.Count; } }
        public IEnumerable<Plane> Planes { get { return this.planes; } }

        public byte[] Board
        {
            get { return this._board; }
        }

        public Field()
        {
            this._board = new byte[]{ 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            this.InitialBoard = new byte[]{ 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            
        }

        public virtual bool CanPlace(Plane pl)
        {
            if (pl.OccupiedCells != null)
            {
                foreach (int index in pl.OccupiedCells)
                {
                    if (this._board[index] > 0 && this._board[index] < 4)
                        return false;
                }
            }
            return true;
        }

        public bool Place(Plane pl)
        {
            foreach (int index in pl.OccupiedCells)
            {
                if (this._board[index] > 0 && this._board[index] < 4)
                    return false;
            }
            foreach (int index in pl.OccupiedCells)
            {
                this._board[index] = Convert.ToByte((this._board[index] & 4) | pl.Fields[index]);   
            }

            this.planes.Add(pl);

            Array.Copy(this._board, this.InitialBoard, 100);

            return true;
        }
        public bool Remove(Plane pl)
        {
            foreach (int index in pl.OccupiedCells)
            {
                if (this._board[index] == 0)
                    return false;
            }
            foreach (int index in pl.OccupiedCells)
            {
                this._board[index] = 0;
            }

            this.planes.Remove(pl);
            return true;
        }

        public byte Shoot(Point Coordinates)
        {
            return this.Shoot(Coordinates.X + Coordinates.Y*9);
        }
        public byte Shoot(int Coordinates)
        {
            if ((this._board[Coordinates] & 4) == 4)
                return 7;
            return (this._board[Coordinates] |= 4);
        }

        public void Clear()
        {
            this._board = new byte[]{ 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            this.InitialBoard = new byte[]{ 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public override string ToString()
        {
            string val= "";
            for (int i = 0; i < 100; i++)
            {
                val += this._board[i] + ", ";

                if (i % 10 == 9)
                    val += "\n";
            }
            return val;
        }

        public byte[] InitialBoard
        { get; private set; }
    }

    public class NetworkField
    {
        public NetworkField()
            : base()
        { }
    }
}
