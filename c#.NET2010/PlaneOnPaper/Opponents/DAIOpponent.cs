using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace PlaneOnPaper
{
    public class DAIOpponent : APossibilityOpponent, IAIOpponent
    {
        public DAIOpponent()
            : base()
        {
            this._my_field = new Field();
            this._my_planes = new List<Plane>();
            this._enemy_field = new Field();
            this.rand = new Random();
        }
        
        Field _enemy_field;
        Random rand;
        protected Field _my_field;
        protected List<Plane> _my_planes;


        public override bool PlacePlanes(int number_of_planes)
        {
            base.PlacePlanes(number_of_planes);
            if (number_of_planes < this._my_planes.Count)
            {
                for (int i = this._my_planes.Count - number_of_planes; i >0 ; i--)
                {
                    this._my_planes.RemoveAt(this._my_planes.Count - 1);
                }
            }

            for (int i = 0; i < 999999999 && number_of_planes != this._my_planes.Count; i++)
            {
                int x, y, d;
                d = this.rand.Next(3) + 1;

                switch (d)
                {
                    case (int)PlaneDirections.North:
                        x = 2 + this.rand.Next(6);
                        y = 1 + this.rand.Next(7);
                        break;

                    case (int)PlaneDirections.South:
                        x = 2 + this.rand.Next(6);
                        y = 2 + this.rand.Next(7);
                        break;

                    case (int)PlaneDirections.East:
                        x = 2 + this.rand.Next(7);
                        y = 2 + this.rand.Next(6);
                        break;

                    default:
                    case (int)PlaneDirections.West:
                        x = 1 + this.rand.Next(7);
                        y = 2 + this.rand.Next(6);
                        break;
                }

                Plane pl = new Plane(Color.Red,x, y, d);
                if (this._my_field.CanPlace(pl))
                {
                    this._my_field.Place(pl);
                    this._my_planes.Add(pl);
                }
            }
            this.PlanesArePlaced = true;
            return number_of_planes == this._my_planes.Count;
        }

        public override byte ShotAt(Point coordinates)
        {
            byte val = this._my_field.Shoot(coordinates.X + coordinates.Y * 10);
            if(val != 7)
            {
                this._display_board[coordinates.X + coordinates.Y * 10] = val;
            }
            
            if (val == 6)
            {
                _shot_down_count++;
            }

            return val;
        }

        public override bool MakeShot()
        {
            int shot_coor = -1;
            int cell_val = 3;
            int possible_plane_index = -1;
            while (cell_val != 0)
            {
                possible_plane_index = this.rand.Next(this.ProbablePlanes.Count);
                shot_coor = this.ProbablePlanes[possible_plane_index].HeadShotIndex;
                cell_val = this._enemy_field.Board[shot_coor];
            }


            this._enemy_field.Board[shot_coor] = this.ShootEnemyHandle(new Point(shot_coor % 10, shot_coor / 10));


            if (this._enemy_field.Board[shot_coor] == 7)
                return false;


            APossibilityOpponent.reevaluatePossibilities(this.ProbablePlanes, this._enemy_field.Board[shot_coor], shot_coor, this._my_planes.Count, this._display_board);

            

            return true;
        }

        public override string Name
        {
            get { return "Даяа /Dumb AI/"; }
        }

        public override bool Defeated
        {
            get { return this._shot_down_count == this._my_planes.Count(); }
        }

        public List<Plane> MyPlanes
        {
            get { return this._my_planes; }
        }

        public override void Clear()
        {
            base.Clear();
            this._my_planes.Clear();
            this._my_field.Clear();
            this._enemy_field.Clear();
        }
    }
}
