using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PlaneOnPaper
{
    

    public enum PlaneDirections { None=0,North, East, South, West };
    

    public partial class frmMain : Form
    {
        private BufferedGraphicsContext context;
        private BufferedGraphics grafx;
        private byte frame_count;
        private System.Windows.Forms.Timer frame_timer;

        private Point _cursor_point;
        public Point CursorPoint
        {
            get { return this._cursor_point; }
            set
            {
                if (value == null || value.X > 9 || value.Y > 19)
                {
                    this._cursor_point = new Point(-1, -1);
                    return;
                }
                //if (value != this._cursor_point)
                //{
                    this._cursor_point = value;
                    if (value.Y < 10)
                        this._cursor_plane.Coordinates = Plane.LimitPlaneCoordinates(this._cursor_point, this._cursor_plane.Direction);
                    else
                    {
                        Point tmp = this._cursor_point;
                        tmp.Y -= 10;
                        this._cursor_plane.Coordinates = Plane.LimitPlaneCoordinates(tmp, this._cursor_plane.Direction);
                    }
                    //this.DrawToBuffer();
                //}
            }
        }
        private Plane _cursor_plane;

        private Field _enemy_ghost_field;
        private List<Plane> _enemy_ghost_planes;


        //private PlaneServer _server;
        //private PlaneClient _client;

        
        public frmMain()
        {
            InitializeComponent();
            this._cursor_point = new Point(-1, -1);
            this._cursor_plane = new Plane() { 
                Direction = (int)PlaneDirections.North, 
                Color = Color.FromArgb(100, Color.Black)
            };

            

            this._enemy_ghost_planes = new List<Plane>();
            this._enemy_ghost_field = new Field();

            Game.InitializeGame();

           
            //this.MouseWheel += new MouseEventHandler(pnlBoard_MouseWheel);
            this.cmbPlaneCount.SelectedIndex = 2;

            //GRAPHIX

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.pnlBoard.Width + 1, this.pnlBoard.Height + 1);
            grafx = context.Allocate(this.pnlBoard.CreateGraphics(),
                 new Rectangle(0, 0, this.pnlBoard.Width, this.pnlBoard.Height));
            // Configure a timer to draw graphics updates.
            frame_timer = new System.Windows.Forms.Timer();
            frame_timer.Interval = 35;
            frame_timer.Tick += new EventHandler(this.OnTimer);
            frame_timer.Start();
            //END GRAPHIX
        }

        


        private void button1_Click(object sender, EventArgs e)
        {
            this.Clear();
            RefreshStates();
            this.Focus();
        }

        private void Clear()
        {
            Game.Reset();
            
            this._enemy_ghost_planes.Clear();
            this._enemy_ghost_field.Clear();

            TCPProtocol.StopTCPListener();
            UDPProtocol.StopUDPListener();
        }

        private void cmbPlaneCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            Game.NumberOfPlanes = this.cmbPlaneCount.SelectedIndex + 1;
            if (Game.GameStatus == GameStatuses.PlacePlanes)
                
            this.Focus();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Game.Me.NameToSet = this.txtName.Text;
            RefreshStates();
            this.Focus();
        }

        private void LockControls()
        {
            if (Game.GameStatus == GameStatuses.PlacePlanes && Game.IsNetwork)
            {
                this.txtName.Enabled = false;
                this.cmbPlaneCount.Enabled = false;
            }
            else
            {
                this.txtName.Enabled = true;
                this.cmbPlaneCount.Enabled = true;
            }
        }

        private void RefreshStates()
        {
            Game.RefreshStates();

            if (Game.GameStatus == GameStatuses.PlacePlanes && Game.Me.PlanesArePlaced)
            {
                this.btnReady.Enabled = true;
                this.btnAI.Enabled = true;
            }
            else
            {
                this.btnReady.Enabled = false;
                this.btnReady.Checked = false;
                this.btnAI.Enabled = false;
                this.btnAI.Checked = false;
            }

        }

        private void btnReady_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnReady.Checked)
            {
                this.btnConnect.Enabled = true;


                try
                {
                    PlaneServer _server = new PlaneServer(Game.NumberOfPlanes, Game.Me.Name);
                }
                catch (SocketException ex)
                {
                    Game.Message = ex.Message;
                }
                //Game.GameStatus = GameStatuses.Ready;

                //temporary computer generated enemy
                
                
                
            }
            LockControls();
            this.Focus();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            PlaneClient _client = new PlaneClient();
            LockControls();
        }


        private void btnAI_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnAI.Checked)
            {
                Game.Opponent = new SambaOpponent();
                Game.GameStatus = GameStatuses.WaitingForSettings;
                RefreshStates();
            }
            LockControls();
        }

        #region User player control
        private void PlacePlane()
        {
            if (CursorPoint.X >= 0 && CursorPoint.X < 10 && CursorPoint.Y >= 0 && CursorPoint.Y < 20)
            {
                Plane pl = new Plane()
                {
                    Coordinates = this._cursor_plane.Coordinates,
                    Direction = this._cursor_plane.Direction
                };
                if (CursorPoint.Y < 10 && Game.Me.MyField.CanPlace(pl) && (Game.GameStatus == GameStatuses.PlacePlanes || Game.GameStatus == GameStatuses.WaitingForSettings))
                {
                    Game.Me.Place(pl);
                    this.RefreshStates();
                }
                else if (CursorPoint.Y > 9 && this._enemy_ghost_field.CanPlace(pl))
                {
                    pl.Color = Color.Green;
                    this._enemy_ghost_planes.Add(pl);
                    this._enemy_ghost_field.Place(pl);
                }
                else if (CursorPoint.Y > 9)
                {
                    Plane[] ghost_temp = new Plane[this._enemy_ghost_planes.Count];
                    this._enemy_ghost_planes.CopyTo(ghost_temp);
                    foreach (Plane ghost in ghost_temp)
                    {
                        if (ghost.Direction == pl.Direction && ghost.Coordinates == pl.Coordinates)
                        {
                            this._enemy_ghost_planes.Remove(ghost);
                            this._enemy_ghost_field.Remove(ghost);
                            break;
                        }
                    }
                }

            }

        }

        private void btnClearGhosts_Click(object sender, EventArgs e)
        {
            this._enemy_ghost_field.Clear();
            this._enemy_ghost_planes.Clear();
            this.Focus();
        }

        #region MouseControls

        private void pnlBoard_MouseEnter(object sender, EventArgs e)
        {
            this.pnlBoard.Focus();
            this.pnlBoard.MouseWheel += new MouseEventHandler(pnlBoard_MouseWheel);
        }

        private void pnlBoard_MouseLeave(object sender, EventArgs e)
        {
            mouseExitedBoard();
            this.pnlBoard.MouseWheel -= new MouseEventHandler(pnlBoard_MouseWheel);
        }

        private void pnlBoard_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                switch (this._cursor_plane.Direction)
                {
                    case (int)PlaneDirections.North:
                    case (int)PlaneDirections.East:
                    case (int)PlaneDirections.South:
                    case (int)PlaneDirections.West:
                        this._cursor_plane.Direction = (int)PlaneDirections.None;
                        break;

                    case (int)PlaneDirections.None:
                        this._cursor_plane.Direction = (int)PlaneDirections.North;
                        break;
                }
                //this.DrawToBuffer();
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (CursorPoint.X >= 0 && CursorPoint.X < 10)
                {
                    if (CursorPoint.Y > 9 && 
                        CursorPoint.Y < 20 && 
                        this._cursor_plane.Direction == (int)PlaneDirections.None &&
                        Game.GameStatus == GameStatuses.GameStarted &&
                        Game.Turn % 2 == Game.MyTurn)

                    {
                        Game.Me.SetShot(new Point(this.CursorPoint.X, this.CursorPoint.Y - 10));
                        
                    }
                    else if (this._cursor_plane.Direction != (int)PlaneDirections.None && ((CursorPoint.Y >= 0 && CursorPoint.Y < 10 && !this.btnReady.Checked)
                        || (CursorPoint.Y > 9 && CursorPoint.Y < 20)))
                    {
                        PlacePlane();
                    }
                }
            }
        }

        private void pnlBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.X > 9 && e.X < 230) && ((e.Y > 9 && e.Y < 230) || (e.Y > 252 && e.Y < 461)))
            {
                this.lblTest.Text = "pos: " + (e.X - 21) / 21 + ", " + (e.Y > 252 ? 10 + (e.Y - 252) / 21 : (e.Y - 21) / 21);
                this.CursorPoint = new Point((e.X - 21) / 21, (e.Y > 252 ? 10 + (e.Y - 252) / 21 : (e.Y - 21) / 21));
            }
            else
            {
                this.mouseExitedBoard();
            }
        }

        private void pnlBoard_MouseWheel(object sender, MouseEventArgs e)
        {
            int change = 100 + (int)this._cursor_plane.Direction - 1;
            change -= e.Delta / 120;
            this._cursor_plane.Direction = Math.Abs(change % 4) + 1;
            pnlBoard_MouseMove(sender, e);
        }

        private void mouseExitedBoard()
        {
            this.lblTest.Text = "";
            this.CursorPoint = new Point(-1, -1);
        }
        #endregion
        #endregion

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    this._cursor_plane.Direction = (int)PlaneDirections.None;
                    break;

                case Keys.Escape:
                    if (this._enemy_ghost_planes.Count > 0)
                    {
                        this._enemy_ghost_field.Remove(this._enemy_ghost_planes[this._enemy_ghost_planes.Count - 1]);
                        this._enemy_ghost_planes.RemoveAt(this._enemy_ghost_planes.Count - 1);
                    }
                    break;
            }
        }

        private void btnRandEnemies_Click(object sender, EventArgs e)
        {
            //generateRandomEnemyPlanes(this.cmbPlaneCount.SelectedIndex + 1);
        }


        #region Graphix
        private void OnTimer(object sender, EventArgs e)
        {
            // Draw randomly positioned ellipses to the buffer.
            DrawToBuffer();

            // If in bufferingMode 2, draw to the form's HDC.
            //if (bufferingMode == 2)
            // Render the graphics buffer to the form's HDC.
            grafx.Render(Graphics.FromHwnd(this.pnlBoard.Handle));
            // If in bufferingMode 0 or 1, draw in the paint method.
            //else
            //this.pnlBoard.Refresh();
        }



        private void pnlBoard_Paint(object sender, PaintEventArgs e)
        {
            grafx.Render(e.Graphics);
        }


        private void DrawToBuffer()
        {
            Graphics g = grafx.Graphics;

            // Clear the graphics buffer every five updates.
            if (++frame_count > 5)
            {
                frame_count = 0;

                grafx.Graphics.FillRectangle(Brushes.Black, 0, 0, this.pnlBoard.Width, this.pnlBoard.Height);

            }

            //draw stuff
            g.DrawImage(global::PlaneOnPaper.Properties.Resources.board, new Point(0, 0));

            if (chkAI.Checked)
            {
                if (Game.Opponent is APossibilityOpponent)
                {
                    foreach (Plane pl in ((APossibilityOpponent)Game.Opponent).PossibleEnemyPlanes)
                    {
                        PaintPlane(ref g, pl, new Point(21, 21));
                    }
                }
            }

            if (this.chkPossibilities.Checked)
            {
                foreach (Plane pl in Game.Me.PossibleEnemyPlanes)
                {
                    PaintPlane(ref g, pl, new Point(21, 210 + 42));
                }
            }

            

            //my planes
            if (chkMine.Checked)
            {
                foreach (Plane pl in Game.Me.MyPlanes)
                {
                    PaintPlane(ref g, pl, new Point(21, 21));
                }
            }



            //draw actual enemy planes
            if (this.chkEnemyPlanes.Checked && 
                (Game.Opponent is IAIOpponent || 
                (Game.Opponent is TCPOpponent && 
                    Game.IsNetwork && 
                    ((TCPOpponent)Game.Opponent).Planes != null)))
            {
                if(Game.Opponent is IAIOpponent)
                {
                    foreach (Plane pl in ((IAIOpponent)Game.Opponent).MyPlanes)
                    {
                        PaintPlane(ref g, pl, new Point(21, 210 + 42));
                    }
                }
                else if(Game.Opponent is TCPOpponent)
                {
                    foreach (Plane pl in ((TCPOpponent)Game.Opponent).Planes)
                    {
                        PaintPlane(ref g, pl, new Point(21, 210 + 42));
                    }
                }
            }

            //ghosts
            foreach (Plane pl in this._enemy_ghost_planes)
            {
                PaintPlane(ref g, pl, new Point(21, 210 + 42));
            }

            //ghost cursor
            if (this._cursor_plane.Direction != (int)PlaneDirections.None && this.CursorPoint.X >= 0 && this.CursorPoint.X < 10 && this.CursorPoint.Y > 9 && this.CursorPoint.Y < 20)
            {
                PaintPlane(ref g, this._cursor_plane, new Point(21, 210 + 42));
            }
            else if (this._cursor_plane.Direction == (int)PlaneDirections.None && this.CursorPoint.X >= 0 && this.CursorPoint.X < 10 && this.CursorPoint.Y > 9 && this.CursorPoint.Y < 20)
            {
                PaintCrosshair(ref g, new Point(this._cursor_point.X, this._cursor_point.Y - 10));
            }

            //my cursor plane
            if ((Game.GameStatus == GameStatuses.PlacePlanes || Game.GameStatus == GameStatuses.WaitingForSettings) && this.CursorPoint.X >= 0 && this.CursorPoint.X < 10 && this.CursorPoint.Y < 10 && this.CursorPoint.Y >= 0)
            {
                //if (this._my_field.CanPlace(this._cursor_plane))
                PaintPlane(ref g, this._cursor_plane, new Point(21, 21));
            }


            


            //shots at me
            for (int i = 0; i < 100; i++)
            {
                if (Game.Me.Board[i] > 3)
                {
                    PaintValue(ref g, Game.Me.Board[i], new Point(i % 10, i / 10), new Point(21, 21));
                }
            }

            //shots at enemy
            if (Game.Opponent != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (Game.Opponent.Board[i] > 3)
                    {
                        PaintValue(ref g, Game.Opponent.Board[i], new Point(i % 10, i / 10), new Point(21, 210 + 42));
                    }
                }
            }


            //turn indicator
            if (Game.GameStatus == GameStatuses.GameStarted && Game.MyTurn >= 0 )
            {
                if (Game.Turn % 2 == Game.MyTurn)
                {
                    PaintTurn(ref g, true, new Point(2, 256));
                }
                else
                {
                    PaintTurn(ref g, false, new Point(2, 25));
                }
            }



            g.DrawString(Game.Message, new Font("Tahoma", 8), Brushes.Black, 10, 467);

            g.DrawString(Game.Me.Name, new Font("Tahoma", 8), Brushes.Blue, 5, 4);
            if (Game.Opponent != null)
            {
                g.DrawString(Game.Opponent.Name, new Font("Tahoma", 8), Brushes.Red, 5, 234);
            }
            if(this.chkPossibilities.Checked)
                g.DrawString("байж болох: " + Game.Me.PossibleEnemyPlanes.Count.ToString(), new Font("Tahoma", 8), Brushes.Orange, 100, 234);
        }

        public static void PaintValue(ref Graphics g, int val, Point coor, Point offset)
        {
            int _x = offset.X + coor.X * 21;
            int _y = offset.Y + coor.Y * 21;

            switch (val)
            {
                case 4:
                    g.DrawRectangle(Pens.Black, new Rectangle(new Point(_x + 6, _y + 5), new Size(1, 1)));
                    g.DrawRectangle(Pens.Black, new Rectangle(new Point(_x + 13, _y + 5), new Size(1, 1)));
                    g.DrawRectangle(Pens.Black, new Rectangle(new Point(_x + 6, _y + 13), new Size(1, 1)));
                    g.DrawRectangle(Pens.Black, new Rectangle(new Point(_x + 13, _y + 13), new Size(1, 1)));

                    g.DrawLine(Pens.Black, new Point(_x + 8, _y + 7), new Point(_x + 13, _y + 12));
                    g.DrawLine(Pens.Black, new Point(_x + 7, _y + 7), new Point(_x + 12, _y + 12));

                    g.DrawLine(Pens.Black, new Point(_x + 8, _y + 12), new Point(_x + 13, _y + 7));
                    g.DrawLine(Pens.Black, new Point(_x + 7, _y + 12), new Point(_x + 12, _y + 7));
                    break;

                case 5:
                    g.DrawRectangle(Pens.Orange, new Rectangle(new Point(_x + 4, _y + 5), new Size(1, 8)));
                    g.DrawRectangle(Pens.Orange, new Rectangle(new Point(_x + 9, _y + 5), new Size(1, 8)));
                    g.DrawRectangle(Pens.Orange, new Rectangle(new Point(_x + 14, _y + 5), new Size(1, 8)));

                    g.DrawLine(Pens.Orange, new Point(_x + 4, _y + 14), new Point(_x + 15, _y + 14));
                    break;

                case 6:
                    g.DrawRectangle(Pens.Red, new Rectangle(new Point(_x + 6, _y + 5), new Size(1, 1)));
                    g.DrawRectangle(Pens.Red, new Rectangle(new Point(_x + 5, _y + 7), new Size(1, 3)));
                    g.DrawRectangle(Pens.Red, new Rectangle(new Point(_x + 6, _y + 11), new Size(1, 1)));
                    g.DrawRectangle(Pens.Red, new Rectangle(new Point(_x + 5, _y + 7), new Size(1, 3)));
                    g.DrawRectangle(Pens.Red, new Rectangle(new Point(_x + 13, _y + 5), new Size(1, 1)));
                    g.DrawRectangle(Pens.Red, new Rectangle(new Point(_x + 13, _y + 11), new Size(1, 1)));

                    g.DrawLine(Pens.Red, new Point(_x + 8, _y + 4), new Point(_x + 13, _y + 4));
                    g.DrawLine(Pens.Red, new Point(_x + 8, _y + 13), new Point(_x + 13, _y + 13));


                    break;
            }
        }

        public static void PaintPlane(ref Graphics g, Plane pl, Point offset)
        {
            Point _plane_coor = pl.Coordinates;
            int _direction = pl.Direction;

            int _x = offset.X + _plane_coor.X * 21;
            int _y = offset.Y + _plane_coor.Y * 21;

            switch (_direction)
            {
                case (int)PlaneDirections.North:
                    Point[] north_points = { 
                        new Point(_x, _y),
                        new Point(_x, _y - 21),
                        new Point(_x + 19, _y - 21),
                        new Point(_x + 19, _y),
                        new Point(_x + 61, _y),
                        new Point(_x + 61, _y + 19),
                        new Point(_x + 19, _y + 19),
                        new Point(_x + 19, _y + 42),
                        new Point(_x + 40, _y + 42),
                        new Point(_x + 40, _y + 61),
                        new Point(_x - 21, _y + 61),
                        new Point(_x - 21, _y + 42),
                        new Point(_x, _y + 42),
                        new Point(_x, _y + 19),
                        new Point(_x - 42, _y + 19),
                        new Point(_x - 42, _y)
                    };

                    g.DrawPolygon(new Pen(pl.Color), north_points);

                    if (pl is ProbablePlane)
                    {
                        g.DrawString(((ProbablePlane)pl).Probability.ToString(), new Font("Tahoma", 6), new SolidBrush(((ProbablePlane)pl).Color), _x, _y - 21);
                    }
                        

                    break;

                case (int)PlaneDirections.South:
                    Point[] south_points = { 
                        new Point(_x, _y),
                        new Point(_x, _y - 23),
                        new Point(_x - 21, _y - 23),
                        new Point(_x - 21, _y - 42),
                        new Point(_x + 40, _y - 42),
                        new Point(_x + 40, _y - 23),
                        new Point(_x + 19, _y - 23),
                        new Point(_x + 19, _y),
                        new Point(_x + 61, _y ),
                        new Point(_x + 61, _y + 19),
                        new Point(_x + 19, _y + 19),
                        new Point(_x + 19, _y + 40),
                        new Point(_x , _y + 40),
                        new Point(_x, _y + 19),
                        new Point(_x - 42, _y + 19),
                        new Point(_x - 42, _y)
                    };

                    g.DrawPolygon(new Pen(pl.Color), south_points);
                    if (pl is ProbablePlane)
                        g.DrawString(((ProbablePlane)pl).Probability.ToString(), new Font("Tahoma", 6), new SolidBrush(((ProbablePlane)pl).Color), _x, _y + 21);

                    break;

                case (int)PlaneDirections.East:
                    Point[] east_points = { 
                        new Point(_x, _y),
                        new Point(_x - 23, _y ),
                        new Point(_x - 23, _y - 21),
                        new Point(_x - 42, _y - 21),
                        new Point(_x - 42, _y + 40),
                        new Point(_x - 23, _y + 40),
                        new Point(_x - 23, _y + 19),
                        new Point(_x, _y + 19),
                        new Point(_x, _y + 61),
                        new Point(_x + 19, _y + 61),
                        new Point(_x + 19, _y + 19),
                        new Point(_x + 40, _y + 19),
                        new Point(_x + 40, _y),
                        new Point(_x + 19, _y),
                        new Point(_x + 19, _y - 42),
                        new Point(_x, _y - 42)
                    };

                    g.DrawPolygon(new Pen(pl.Color), east_points);
                    if (pl is ProbablePlane)
                        g.DrawString(((ProbablePlane)pl).Probability.ToString(), new Font("Tahoma", 6), new SolidBrush(((ProbablePlane)pl).Color), _x + 21, _y);

                    break;

                case (int)PlaneDirections.West:
                    Point[] west_points = { 
                        new Point(_x, _y),
                        new Point(_x - 21, _y),
                        new Point(_x - 21, _y + 19),
                        new Point(_x, _y + 19),
                        new Point(_x, _y + 61),
                        new Point(_x + 19, _y + 61),
                        new Point(_x + 19, _y + 19),
                        new Point(_x + 42, _y + 19),
                        new Point(_x + 42, _y + 40),
                        new Point(_x + 61, _y + 40),
                        new Point(_x + 61, _y - 21),
                        new Point(_x + 42, _y - 21),
                        new Point(_x + 42, _y),
                        new Point(_x + 19, _y),
                        new Point(_x + 19, _y - 42),
                        new Point(_x, _y - 42)
                    };
                    g.DrawPolygon(new Pen(pl.Color), west_points);
                    if (pl is ProbablePlane)
                        g.DrawString(((ProbablePlane)pl).Probability.ToString(), new Font("Tahoma", 6), new SolidBrush(((ProbablePlane)pl).Color), _x - 21, _y);

                    break;
            }
        }

        public static void PaintTurn(ref Graphics g, bool _is_mine, Point coor)
        {
            int _x = coor.X;
            int _y = coor.Y;
            Pen color;
            if (_is_mine)
                color = Pens.Blue;
            else
                color = Pens.Red;


            g.DrawLine(color, new Point(_x, _y), new Point(_x + 12, _y));
            g.DrawLine(color, new Point(_x, _y + 1), new Point(_x + 13, _y + 1));
            g.DrawLine(color, new Point(_x, _y + 2), new Point(_x + 14, _y + 2));
            g.DrawLine(color, new Point(_x, _y + 3), new Point(_x + 15, _y + 3));
            g.DrawLine(color, new Point(_x, _y + 4), new Point(_x + 15, _y + 4));
            g.DrawLine(color, new Point(_x, _y + 5), new Point(_x + 14, _y + 5));
            g.DrawLine(color, new Point(_x, _y + 6), new Point(_x + 13, _y + 6));
            g.DrawLine(color, new Point(_x, _y + 7), new Point(_x + 12, _y + 7));

        }

        public static void PaintCrosshair(ref Graphics g, Point cursor)
        {
            g.DrawRectangle(Pens.DarkRed, new Rectangle(new Point(21 + 21*cursor.X + 4, 210+42 + 21*cursor.Y + 9), new Size(4,2)));
            g.DrawRectangle(Pens.DarkRed, new Rectangle(new Point(21 + 21*cursor.X + 12, 210+42 + 21*cursor.Y + 9), new Size(4,2)));
            g.DrawRectangle(Pens.DarkRed, new Rectangle(new Point(21 + 21*cursor.X + 9, 210+42 + 21*cursor.Y + 4), new Size(2,4)));
            g.DrawRectangle(Pens.DarkRed, new Rectangle(new Point(21 + 21*cursor.X + 9, 210+42 + 21*cursor.Y + 12), new Size(2,4)));
            
        }

        #endregion

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Game.StopGame();
            frame_timer.Stop();
            frame_timer.Dispose();

            UDPProtocol.StopUDPListener();
            TCPProtocol.StopTCPListener();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.txtName.Text = "Player";
        }

        private void pnlBoard_Click(object sender, EventArgs e)
        {
            this.pnlBoard.Focus();
        }


        #region Server
        

        #endregion

    }
}
