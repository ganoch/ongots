using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaneOnPaper
{
    public class Plane
    {
        private int _dir;
        private Point _coor;
        private byte[] _fields; //<coordinates,value>
        private int[] _occupied_cells;
        private bool _field_is_set = false;

        public virtual Color Color
        {
            get;
            set;
        }

        
        public Plane(Color color, int x, int y, int dir)
            : this( color, new Point(x,y),dir)
        {
        }

        public Plane(Color color, Point coor, int dir)
        {
            this.Direction = dir;
            this.Coordinates = coor;
            this.Color = color;
        }

        public Plane()
            : this(Color.Blue, 0, 0, 0) { }



        public int Direction
        {
            get { return this._dir; }
            set {
                if (value != this._dir)
                {
                    this._dir = value;
                    if (this._coor != null)
                    {
                        this._coor = LimitPlaneCoordinates(this._coor, value);
                    }
                    if(this._dir != (int)PlaneDirections.None)
                        calculateFields();
                }
                
            }
        }

        public Point Coordinates
        {
            get { return this._coor; }
            set {
                value = LimitPlaneCoordinates(value, this._dir);
                if (value != this._coor)
                {
                    this._coor = value;
                    if (this._dir != (int)PlaneDirections.None)
                        calculateFields();
                }
            }
        }

        public byte[] Fields
        {
            get
            {
                if (!this._field_is_set)
                {
                    calculateFields();
                }

                return this._fields;
            }
            set
            {
                this._fields = value;
            }
        }

        public int[] OccupiedCells
        {
            get
            {
                if (!this._field_is_set)
                {
                    calculateFields();
                }
                return this._occupied_cells;
            }
        }

        protected virtual void calculateFields()
        {
            this._fields = new byte[100];
            switch (this.Direction)
            {
                case (int)PlaneDirections.North:
                    this._fields[(this.Coordinates.Y - 1) * 10 + this.Coordinates.X] = 2;

                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 2] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 1] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 1] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 2] = 1;

                    this._fields[(this.Coordinates.Y + 1) * 10 + this.Coordinates.X] = 1;

                    this._fields[(this.Coordinates.Y + 2) * 10 + this.Coordinates.X - 1] = 1;
                    this._fields[(this.Coordinates.Y + 2) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y + 2) * 10 + this.Coordinates.X + 1] = 1;

                    this._occupied_cells = new int[]
                    {
                        (this.Coordinates.Y - 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 2,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 1,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 1,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 2,
                        (this.Coordinates.Y + 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y + 2) * 10 + this.Coordinates.X - 1,
                        (this.Coordinates.Y + 2) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y + 2) * 10 + this.Coordinates.X + 1
                    };
                    break;

                case (int)PlaneDirections.South:
                    this._fields[(this.Coordinates.Y + 1) * 10 + this.Coordinates.X] = 2;

                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 2] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 1] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 1] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 2] = 1;

                    this._fields[(this.Coordinates.Y - 1) * 10 + this.Coordinates.X] = 1;

                    this._fields[(this.Coordinates.Y - 2) * 10 + this.Coordinates.X - 1] = 1;
                    this._fields[(this.Coordinates.Y - 2) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y - 2) * 10 + this.Coordinates.X + 1] = 1;


                    this._occupied_cells = new int[]
                    {
                        (this.Coordinates.Y + 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 2,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 1,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 1,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 2,
                        (this.Coordinates.Y - 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y - 2) * 10 + this.Coordinates.X - 1,
                        (this.Coordinates.Y - 2) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y - 2) * 10 + this.Coordinates.X + 1
                    };
                    break;

                case (int)PlaneDirections.East:
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 1] = 2;

                    this._fields[(this.Coordinates.Y - 2) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y - 1) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y + 1) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y + 2) * 10 + this.Coordinates.X] = 1;

                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 1] = 1;

                    this._fields[(this.Coordinates.Y - 1) * 10 + this.Coordinates.X - 2] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 2] = 1;
                    this._fields[(this.Coordinates.Y + 1) * 10 + this.Coordinates.X - 2] = 1;

                    this._occupied_cells = new int[]
                    {
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 1,
                        (this.Coordinates.Y - 2) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y - 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y + 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y + 2) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 1,
                        (this.Coordinates.Y - 1) * 10 + this.Coordinates.X - 2,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 2,
                        (this.Coordinates.Y + 1) * 10 + this.Coordinates.X - 2
                    };
                    break;

                case (int)PlaneDirections.West:
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X - 1] = 2;

                    this._fields[(this.Coordinates.Y - 2) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y - 1) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y + 1) * 10 + this.Coordinates.X] = 1;
                    this._fields[(this.Coordinates.Y + 2) * 10 + this.Coordinates.X] = 1;

                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 1] = 1;

                    this._fields[(this.Coordinates.Y - 1) * 10 + this.Coordinates.X + 2] = 1;
                    this._fields[(this.Coordinates.Y) * 10 + this.Coordinates.X + 2] = 1;
                    this._fields[(this.Coordinates.Y + 1) * 10 + this.Coordinates.X + 2] = 1;

                    this._occupied_cells = new int[]
                    {
                        (this.Coordinates.Y) * 10 + this.Coordinates.X - 1,
                        (this.Coordinates.Y - 2) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y - 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y + 1) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y + 2) * 10 + this.Coordinates.X,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 1,
                        (this.Coordinates.Y - 1) * 10 + this.Coordinates.X + 2,
                        (this.Coordinates.Y) * 10 + this.Coordinates.X + 2,
                        (this.Coordinates.Y + 1) * 10 + this.Coordinates.X + 2
                    };
                    break;
            }
            this._field_is_set = true;
        }

        public static Point LimitPlaneCoordinates(Point current_coordinates, int direction)
        {
            Point ret_val = current_coordinates;
            switch (direction)
            {
                case (int)PlaneDirections.North:
                    if (current_coordinates.X < 2)
                        ret_val.X = 2;
                    else if (current_coordinates.X > 7)
                        ret_val.X = 7;

                    if (current_coordinates.Y < 1)
                        ret_val.Y = 1;
                    else if (current_coordinates.Y > 7)
                        ret_val.Y = 7;
                    break;

                case (int)PlaneDirections.South:
                    if (current_coordinates.X < 2)
                        ret_val.X = 2;
                    else if (current_coordinates.X > 7)
                        ret_val.X = 7;

                    if (current_coordinates.Y < 2)
                        ret_val.Y = 2;
                    else if (current_coordinates.Y > 8)
                        ret_val.Y = 8;
                    break;

                case (int)PlaneDirections.East:
                    if (current_coordinates.X < 2)
                        ret_val.X = 2;
                    else if (current_coordinates.X > 8)
                        ret_val.X = 8;

                    if (current_coordinates.Y < 2)
                        ret_val.Y = 2;
                    else if (current_coordinates.Y > 7)
                        ret_val.Y = 7;
                    break;

                case (int)PlaneDirections.West:
                    if (current_coordinates.X < 1)
                        ret_val.X = 1;
                    else if (current_coordinates.X > 7)
                        ret_val.X = 7;

                    if (current_coordinates.Y < 2)
                        ret_val.Y = 2;
                    else if (current_coordinates.Y > 7)
                        ret_val.Y = 7;
                    break;
            }

            return ret_val;
        }

        public static bool isOverlapping(Plane p0, Plane p1)
        {
            foreach (int index in p0.OccupiedCells)
            {
                if (Array.IndexOf(p1.OccupiedCells, index) > -1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isOverlapping(Plane pl)
        {
            return Plane.isOverlapping(this, pl);
        }
    }
}
