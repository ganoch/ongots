using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace PlaneOnPaper
{
    public class UserOpponent : APossibilityOpponent
    {
        private string _name;
        protected Field _my_field;
        public List<Plane> _my_planes;
        public Point _current_shot;
        protected int _planes_to_place;

        public UserOpponent()
            : base() {
            this._my_field = new Field();
            this._my_planes = new List<Plane>();
            this._current_shot = NoShot;
        }

        public override bool PlacePlanes(int number_of_planes)
        {
            base.PlacePlanes(number_of_planes);
            this._planes_to_place = number_of_planes;
            return this.PlanesArePlaced;
        }

        public override byte ShotAt(Point coordinates)
        {
            this._display_board[coordinates.X + coordinates.Y * 10] = this._my_field.Shoot(coordinates.X + coordinates.Y * 10);
            if (this._display_board[coordinates.X + coordinates.Y * 10] == 6)
            {
                this._shot_down_count++;
            }

            return this._display_board[coordinates.X + coordinates.Y * 10];
        }

        public override bool MakeShot()
        {
            if (!AOpponent.NoShot.Equals(this._current_shot))
            {
                byte shot_returned = this.ShootEnemyHandle(this._current_shot);
                

                if (shot_returned == 7)
                {
                    this._current_shot = AOpponent.NoShot;
                    System.Diagnostics.Debug.WriteLine("Wrong shot");
                    return false;
                }
                this._enemy_display_board[this._current_shot.X + this._current_shot.Y*10] = shot_returned;

                APossibilityOpponent.reevaluatePossibilities(this.ProbablePlanes, shot_returned, (this._current_shot.X + this._current_shot.Y * 10), this._my_planes.Count, this._enemy_display_board);
                this._current_shot = AOpponent.NoShot;
                
                return true;
            }
            return false;
        }

        public override string Name
        {
            get { return this._name; }
        }

        public string NameToSet
        {
            set
            {
                this._name = value;
            }
        }

        public bool Place(Plane pl)
        {
            if (this._my_field.CanPlace(pl))
            {
                this._my_planes.Add(pl);
                this._my_field.Place(pl);
                return true;
            }
            return false;
        }

        public Field MyField
        {
            get { return this._my_field; }
        }

        public List<Plane> MyPlanes
        {
            get { return this._my_planes; }
        }

        public override bool Defeated
        {
            get { return this._shot_down_count == this._my_planes.Count(); }
        }

        public void SetShot(Point p)
        {
            this._current_shot = p;
        }


        public override void Clear()
        {
            base.Clear();
            this._my_planes.Clear();
            this._my_field.Clear();
            this._current_shot = AOpponent.NoShot;
        }

        public override bool PlanesArePlaced
        {
            get
            {
                base.PlanesArePlaced = (this._my_planes.Count == this._planes_to_place);
                return this._my_planes.Count == this._planes_to_place;
            }
            set
            {
                base.PlanesArePlaced = value;
            }
        }

        public byte[] InitialBoard { get { return this.MyField.InitialBoard; } }

    }
}
