using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaneOnPaper
{
    public class ProbablePlane : Plane, IComparable, ICloneable
    {
        public ProbablePlane(Color color, int x, int y, int dir)
            : base( color,  x,  y,  dir)
            
        {
        }

        public ProbablePlane(Color color, Point coor, int dir) 
            : base( color,  coor,  dir)
        {
            this.Direction = dir;
            this.Coordinates = coor;
            this.Color = color;
        }

        public ProbablePlane()
            : this(Color.Blue, 0, 0, 0) { }

        int headshot_index = -1;
        protected override void calculateFields()
        {
            base.calculateFields();

            switch (this.Direction)
            {
                case (int)PlaneDirections.North:
                    this.headshot_index = (this.Coordinates.Y - 1) * 10 + this.Coordinates.X;
                    break;

                case (int)PlaneDirections.South:
                    this.headshot_index = (this.Coordinates.Y + 1) * 10 + this.Coordinates.X;
                    break;

                case (int)PlaneDirections.East:
                    this.headshot_index = (this.Coordinates.Y) * 10 + this.Coordinates.X + 1;
                    break;

                case (int)PlaneDirections.West:
                    this.headshot_index = (this.Coordinates.Y) * 10 + this.Coordinates.X - 1;
                    break;
            }
        }

        private float _probability;
        public float Probability
        {
            get { return this._probability; }
            set
            {
                if (this._probability != value)
                {
                    this._probability = value;
                    this._color = Color.FromArgb(63 + Convert.ToInt32((float)96 * Math.Pow(this.Probability, 1.3)), 255, Convert.ToInt32((float)170 * ((float)1 - this.Probability)), 0);
                }
            }
        }

        private Color _color;
        public override Color Color
        {
            get
            {
                //*
                if (this._color == null)
                    this._color = Color.FromArgb(this.BeingEvaluated?255:(63 + Convert.ToInt32((float)96 * Math.Pow(this.Probability,1.3))), 255, Convert.ToInt32((float)170 * ((float)1 - this.Probability)), 0);
                return this._color;
                /*/
                return base.Color;
                //*/
            }
            set
            {
                base.Color = value;
                this._color = Color.FromArgb(63 + Convert.ToInt32((float)96 * Math.Pow(this.Probability, 1.3)), 255, Convert.ToInt32((float)170 * ((float)1 - this.Probability)), 0);
            }
        }


        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ProbablePlane otherProbablePlane = obj as ProbablePlane;
            if (otherProbablePlane != null)
                return this.Probability.CompareTo(otherProbablePlane.Probability);
            else
                throw new ArgumentException("Object is not a ProbablePlane");
        }

        public int HeadShotIndex 
        { 
            get 
            {
                if (this.headshot_index < 0)
                    this.calculateFields();
                return this.headshot_index;
            }
        }

        private bool checked_shot = false;
        private bool is_shot_at = false;

        public bool IsShot
        {
            get
            {
                if (!this.checked_shot)
                {
                    this.checked_shot = true;
                    foreach (int index in this.OccupiedCells)
                    {
                        if (this.is_shot_at = (this.Fields[index] > 4) )
                        {
                            break;
                        }
                    }
                }

                return this.is_shot_at;
            }
        }

        private bool _being_evaluated;
        public bool BeingEvaluated
        {
            get
            {
                return this._being_evaluated;
            }

            set
            {
                this._being_evaluated = value;
            }
        }

        public object Clone()
        {
            ProbablePlane toReturn = new ProbablePlane(this.Color, this.Coordinates.X, this.Coordinates.Y, this.Direction);
            Array.Copy(this.Fields, toReturn.Fields, this.Fields.Length);
            toReturn.Probability = this.Probability;
            return (object)toReturn;
        }
    }
}
