using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaneOnPaper
{
    public abstract class AOpponent
    {

        protected byte[] _display_board;
        protected byte[] _enemy_display_board;
        protected int _shot_down_count;
        protected GameStatuses _game_status;

        public AOpponent()
        {
            this._display_board = new byte[100];
            this._enemy_display_board = new byte[100];

        }

        /// <summary>
        /// Онгоцуудыг байрлуулах функц, байрлуулахдаа байрлуулсан эсэхээ тодорхойлох функц
        /// </summary>
        /// <param name="number_of_planes"></param>
        /// <returns></returns>
        public abstract bool PlacePlanes(int number_of_planes);

        public event EventHandler PlanesPlaced;

        protected virtual void OnPlanesPlaced(EventArgs e)
        {
            if (PlanesPlaced != null)
                PlanesPlaced(this, e);
        }

        /// <summary>
        /// буудагдах үед дуудагдах method,
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public abstract byte ShotAt(Point coordinates);


        /// <summary>
        /// буудалтыг гүйцэтгэх функц
        /// </summary>
        public abstract bool MakeShot();

        /// <summary>
        /// буудах үед ашиглах delegate буюу буудах функцын хэлбэр
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public delegate byte ShootEnemy(Point coordinates);

        /// <summary>
        /// буудах үед ашиглах handle буюу буудах функцын төлөөлөгч хувьсагч, <seealso cref="MakeShot"/>
        /// </summary>
        public ShootEnemy ShootEnemyHandle;


        /// <summary>
        /// хэвлэгдэх самбар
        /// </summary>
        public virtual byte[] Board
        {
            get
            {
                return this._display_board;
            }
        }


        /// <summary>
        /// хэвлэгдэх тоглогчийн нэр
        /// </summary>
        public abstract string Name
        {
            get;
        }

        protected bool _finished_placing_planes;

        public virtual bool PlanesArePlaced
        {
            set { 
                bool old_value = this._finished_placing_planes;
                this._finished_placing_planes = value;
                if (value && !old_value)
                    this.OnPlanesPlaced(EventArgs.Empty);
            }
            get { return this._finished_placing_planes; }
        }

        public abstract bool Defeated { get; }

        public virtual void Clear()
        {
            this._display_board = new byte[100];
            this._enemy_display_board = new byte[100];
            this._finished_placing_planes = false;
            this._shot_down_count = 0;
        }

        public virtual GameStatuses GameStatus
        {
            set { this._game_status = value; }
            get { return this._game_status; }
        }

        public static Point NoShot = new Point(-1, -1);
    }
}
